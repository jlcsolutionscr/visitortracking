using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using jlcsolutionscr.com.visitortracking.webapi.customclasses;
using jlcsolutionscr.com.visitortracking.webapi.dataaccess;
using jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain;

namespace jlcsolutionscr.com.visitortracking.webapi.services
{
    public class VisitorTrackingService : IDisposable
    {
        private readonly AppSettings _settings;
        private static CultureInfo provider = CultureInfo.InvariantCulture;
        private static string strFormat = "dd/MM/yyyy HH:mm:ss";

        public VisitorTrackingService(AppSettings settings)
        {
            _settings = settings;
        }

        private string GenerateAuthorization()
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                string strGuid = Guid.NewGuid().ToString();
                try
                {
                    
                    AuthorizationEntry entry = new AuthorizationEntry
                    {
                        Id = strGuid,
                        EmitedAt = DateTime.UtcNow
                    };
                    dbContext.AuthorizationEntryRepository.Add(entry);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                string encryptedToken = Utilities.EncryptString(strGuid);
                return encryptedToken;
            }
        }

        public void ValidateAccessCode(string token)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    string deencryptedToken = Utilities.DecryptString(token);
                    AuthorizationEntry entry = dbContext.AuthorizationEntryRepository.Where(x => x.Id == deencryptedToken).FirstOrDefault();
                    if (entry == null) throw new Exception("La sessión del usuario no es válida. Debe reiniciar su sesión.");
                    if (entry.EmitedAt < DateTime.UtcNow.AddHours(-12))
                    {
                        dbContext.RemoveNotify(entry);
                        dbContext.Commit();
                        throw new Exception("La sessión del usuario se encuentra expirada. Debe reiniciar su sesión.");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string GetLatestAppVersion()
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Parameter param = dbContext.ParameterRepository.FirstOrDefault(x => x.Id == 1);
                    if (param == null) throw new Exception("El sistema no posee el parámetro de la versión del app móvil.");
                    return param.Value;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void RemoveInvalidEntries()
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                string strGuid = Guid.NewGuid().ToString();
                try
                {
                    DateTime detMaxDate = DateTime.UtcNow;
                    var list = dbContext.AuthorizationEntryRepository.Where(x => x.EmitedAt < detMaxDate).ToList();
                    foreach(AuthorizationEntry entry in list)
                    {
                        dbContext.RemoveNotify(entry);
                        dbContext.Commit();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Session UserLogin(string username, string password, string identifier)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    User user = null;
                    Company company = null;
                    if (username.ToUpper() == "ADMIN" || username.ToUpper() == "MOBILEAPP")
                    {
                        user = dbContext.UserRepository.FirstOrDefault(x => x.Username == username.ToUpper());
                    }
                    else
                    {
                        company = dbContext.CompanyRepository.FirstOrDefault(x => x.Identifier == identifier);
                        if (company == null) throw new Exception("La identificación suministrada no pertenece a ninguna empresa registrada en el sistema. Por favor verifique la información suministrada.");
                        if (company.ExpiresAt < DateTime.UtcNow.AddHours(company.UtcTimeFactor)) throw new Exception("La vigencia del plan de facturación ha expirado. Por favor, pongase en contacto con su proveedor de servicio.");
                        user = dbContext.UserRepository.FirstOrDefault(x => x.Username == username.ToUpper() && x.Identifier == identifier);
                    }
                    if (user == null) throw new Exception("Usuario no registrado en la empresa suministrada. Por favor verifique la información suministrada.");
                    if (user.Password != password) throw new Exception("Los credenciales suministrados no son válidos. Verifique los credenciales suministrados.");
                    List<RolePerUser> roles = dbContext.RolePerUserRepository.Where(x => x.UserId == user.Id).ToList();
                    Session session = new Session();
                    session.CompanyId = company == null ? -1 : company.Id;
                    session.CompanyName = company == null ? null : company.CompanyName;
                    session.CompanyIdentifier = company == null ? null : company.Identifier;
                    roles.ForEach(item => {
                        RoleItem role = new RoleItem(item.RoleId, item.UserId);
                        session.RolePerUser.Add(role);
                    });
                    string token = GenerateAuthorization();
                    session.Token = token;
                    return session;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<IdDescList> GetCompanyList()
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                List<IdDescList> results = new List<IdDescList>();
                try
                {
                    List<Company> list = dbContext.CompanyRepository.ToList();
                    foreach(var company in list)
                    {
                        IdDescList item = new IdDescList(company.Id, company.CompanyName);
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Company GetCompany(int id)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Company entity = dbContext.CompanyRepository.FirstOrDefault(x => x.Id == id);
                    return entity;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string AddCompany(Company entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    dbContext.CompanyRepository.Add(entity);
                    dbContext.Commit();
                    return entity.Id.ToString();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void UpdateCompany(Company entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    dbContext.ChangeNotify(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<IdDescList> GetUserList(string identifier)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                List<IdDescList> results = new List<IdDescList>();
                try
                {
                    List<User> list = dbContext.UserRepository.Where(x => x.Identifier == identifier).ToList();
                    foreach(var user in list)
                    {
                        IdDescList item = new IdDescList(user.Id, user.Username);
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public User GetUser(int id)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    User entity = dbContext.UserRepository.FirstOrDefault(x => x.Id == id);
                    if (entity != null) {
                        List<IdDescList> roleList = new List<IdDescList>();
                        List<RolePerUser> list = dbContext.RolePerUserRepository.Where(x => x.UserId == entity.Id).ToList();
                        list.ForEach(item => {
                            Role role = dbContext.RoleRepository.Find(item.RoleId);
                            roleList.Add(new IdDescList(role.Id, role.Description));
                        });
                        entity.RoleList = roleList;
                    }
                    return entity;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string AddUser(User entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    entity.Username = entity.Username.ToUpper();
                    if (entity.Username == "ADMIN" || entity.Username == "MOBILEAPP") throw new Exception("El código de usuario ingresado no está disponible");
                    User existing = dbContext.UserRepository.AsNoTracking().FirstOrDefault(x => x.Identifier == entity.Identifier && x.Username == entity.Username);
                    if (existing != null) throw new Exception("El código de usuario ya se encuentra registrado. No es posible agregar el registro.");
                    dbContext.UserRepository.Add(entity);
                    entity.RoleList.ForEach(role => {
                        RolePerUser rolePerUser = new RolePerUser();
                        rolePerUser.User = entity;
                        rolePerUser.RoleId = role.Id;
                        dbContext.RolePerUserRepository.Add(rolePerUser);
                    });
                    dbContext.Commit();
                    return entity.Id.ToString();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void UpdateUser(User entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    if (entity.Password == "")
                    {
                        string password = dbContext.UserRepository.AsNoTracking().FirstOrDefault(x => x.Id == entity.Id).Password;
                        entity.Password = password;
                    }
                    dbContext.ChangeNotify(entity);
                    List<RolePerUser> list = dbContext.RolePerUserRepository.Where(x => x.UserId == entity.Id).ToList();
                    list.ForEach(item => {
                        dbContext.RemoveNotify(item);
                    });
                    entity.RoleList.ForEach(role => {
                        RolePerUser rolePerUser = new RolePerUser();
                        rolePerUser.UserId = entity.Id;
                        rolePerUser.RoleId = role.Id;
                        dbContext.RolePerUserRepository.Add(rolePerUser);
                    });
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<IdDescList> GetRoleList()
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                List<IdDescList> results = new List<IdDescList>();
                try
                {
                    List<Role> list = dbContext.RoleRepository.ToList();
                    foreach(var role in list)
                    {
                        IdDescList item = new IdDescList(role.Id, role.Description);
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<IdDescList> GetBranchList(int companyId)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                List<IdDescList> results = new List<IdDescList>();
                try
                {
                    List<Branch> list = dbContext.BranchRepository.Where(x => x.CompanyId == companyId).ToList();
                    foreach(var role in list)
                    {
                        IdDescList item = new IdDescList(role.Id, role.Description);
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Branch GetBranch(int companyId, int id)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Branch entity = dbContext.BranchRepository.FirstOrDefault(x => x.CompanyId == companyId && x.Id == id);
                    return entity;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string AddBranch(Branch entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    entity.AccessCode = Guid.NewGuid().ToString();
                    dbContext.BranchRepository.Add(entity);
                    dbContext.Commit();
                    return entity.Id.ToString();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void UpdateBranch(Branch entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    dbContext.ChangeNotify(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public Customer GetCustomer(int id)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Customer entity = dbContext.CustomerRepository.FirstOrDefault(x => x.Id == id);
                    return entity;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string AddCustomer(Customer entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    dbContext.CustomerRepository.Add(entity);
                    dbContext.Commit();
                    return entity.Id.ToString();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void UpdateCustomer(Customer entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    dbContext.ChangeNotify(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<IdDescList> GetEmployeeList(int companyId)
        {
            List<IdDescList> results = new List<IdDescList>();
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Employee> list = dbContext.EmployeeRepository.Where(x => x.CompanyId == companyId).ToList();
                    foreach(var employee in list)
                    {
                        IdDescList item = new IdDescList(employee.Id, employee.Name);
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<IdDescList> GetActiveEmployeeList(int companyId)
        {
            List<IdDescList> results = new List<IdDescList>();
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Employee> list = dbContext.EmployeeRepository.Where(x => x.CompanyId == companyId && x.Active).ToList();
                    foreach(var employee in list)
                    {
                        IdDescList item = new IdDescList(employee.Id, employee.Name);
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Employee GetEmployee(int id)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Employee entity = dbContext.EmployeeRepository.FirstOrDefault(x => x.Id == id);
                    return entity;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string AddEmployee(Employee entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Employee> list = dbContext.EmployeeRepository.Where(x => x.CompanyId == entity.CompanyId).ToList();
                    int maxId = 0;
                    if (list.Count > 0) maxId = dbContext.EmployeeRepository.Where(x => x.CompanyId == entity.CompanyId).Max(x => x.Id);
                    entity.Id = maxId + 1;
                    dbContext.EmployeeRepository.Add(entity);
                    dbContext.Commit();
                    return entity.Id.ToString();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void UpdateEmployee(Employee entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    dbContext.ChangeNotify(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<IdDescList> GetServiceList(int companyId)
        {
            List<IdDescList> results = new List<IdDescList>();
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Service> list = dbContext.ServiceRepository.Where(x => x.CompanyId == companyId).ToList();
                    foreach(var employee in list)
                    {
                        IdDescList item = new IdDescList(employee.Id, employee.Description);
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<IdDescList> GetActiveServiceList(int companyId)
        {
            List<IdDescList> results = new List<IdDescList>();
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Service> list = dbContext.ServiceRepository.Where(x => x.CompanyId == companyId && x.Active).ToList();
                    foreach(var employee in list)
                    {
                        IdDescList item = new IdDescList(employee.Id, employee.Description);
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Service GetService(int id)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Service entity = dbContext.ServiceRepository.FirstOrDefault(x => x.Id == id);
                    return entity;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string AddService(Service entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Service> list = dbContext.ServiceRepository.Where(x => x.CompanyId == entity.CompanyId).ToList();
                    int maxId = 0;
                    if (list.Count > 0) maxId = dbContext.ServiceRepository.Where(x => x.CompanyId == entity.CompanyId).Max(x => x.Id);
                    entity.Id = maxId + 1;
                    dbContext.ServiceRepository.Add(entity);
                    dbContext.Commit();
                    return entity.Id.ToString();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void UpdateService(Service entity)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    dbContext.ChangeNotify(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public Branch GetBranchByCode(string accessCode)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Branch branch = dbContext.BranchRepository.FirstOrDefault(x => x.AccessCode == accessCode);
                    if (branch == null) throw new Exception("No se logró obtener la información de la sucursal que envia la solicitud.");
                    if (!branch.Active) throw new Exception("La sucursal se encuentra inactiva en el sistema. Consulte con su proveedor del servicio.");
                    Company company = dbContext.CompanyRepository.Find(branch.CompanyId);
                    if (company.ExpiresAt < DateTime.UtcNow.AddHours(company.UtcTimeFactor)) throw new Exception("La empresa se encuentra inhabilitada en el sistema. Consulte con su proveedor del servicio.");
                    return branch;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<ActivityList> GetPendingRegistryList(int companyId)
        {
            List<ActivityList> results = new List<ActivityList>();
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    var list = dbContext.CustomerRepository.Join(dbContext.RegistryRepository, x => x.Id, y => y.CustomerId, (x, y) => new { x, y })
                        .Where(x => x.y.CompanyId == companyId && x.y.Status == StaticStatus.Pending)
                        .Select(x => new { Id = x.y.Id, Name = x.x.Name, RegisterDate = x.y.RegisterDate }).ToList();   
                    foreach(var entry in list)
                    {
                        ActivityList item = new ActivityList(entry.Id, entry.Name, entry.RegisterDate.ToString(strFormat), "");
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<IdDescList> GetRegisteredCustomerList(string deviceId, string accessCode)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                List<IdDescList> results = new List<IdDescList>();
                try
                {
                    Branch branch = dbContext.BranchRepository.FirstOrDefault(x => x.AccessCode == accessCode);
                    if (branch == null) throw new Exception("No se logró obtener la información de la sucursal que envia la solicitud.");
                    if (!branch.Active) throw new Exception("La sucursal se encuentra inactiva en el sistema. Consulte con su proveedor del servicio.");
                    Company company = dbContext.CompanyRepository.Find(branch.CompanyId);
                    if (company.ExpiresAt < DateTime.UtcNow.AddHours(company.UtcTimeFactor)) throw new Exception("La empresa se encuentra inhabilitada en el sistema. Consulte con su proveedor del servicio.");
                    List<Customer> list = dbContext.CustomerRepository.Join(dbContext.RegistryRepository, x => x.Id, y => y.CustomerId, (x, y) => new { x, y })
                        .Where(x => x.y.DeviceId == deviceId && x.y.CompanyId == company.Id && x.y.Status == StaticStatus.Active)
                        .Select(x => x.x).ToList();
                    foreach(var entry in list)
                    {
                        IdDescList item = new IdDescList(entry.Id, entry.Name);
                        results.Add(item);
                    }
                    return results;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void CustomerRegistry(string deviceId, string accessCode, string name, string identifier, string birthday, string address, string mobileNumber, string email)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Branch branch = dbContext.BranchRepository.FirstOrDefault(x => x.AccessCode == accessCode);
                    if (branch == null) throw new Exception("No se logró obtener la información de la sucursal que envia la solicitud.");
                    if (!branch.Active) throw new Exception("La sucursal se encuentra inactiva en el sistema. Consulte con su proveedor del servicio.");
                    Company company = dbContext.CompanyRepository.Find(branch.CompanyId);
                    if (company.ExpiresAt < DateTime.UtcNow.AddHours(company.UtcTimeFactor)) throw new Exception("La empresa se encuentra inhabilitada en el sistema. Consulte con su proveedor del servicio.");
                    DateTime datBirthday = DateTime.ParseExact(birthday + " 00:00:01", strFormat, provider);
                    Customer customer = dbContext.CustomerRepository.FirstOrDefault(x => x.Identifier == identifier);
                    if (customer == null)
                    {
                        customer = new Customer();
                        customer.Name = name;
                        customer.Identifier = identifier;
                        customer.Birthday = datBirthday;
                        customer.Address = address;
                        customer.MobileNumber = mobileNumber;
                        customer.Email = email;
                        dbContext.CustomerRepository.Add(customer);
                    }
                    else
                    {
                        customer.Name = name;
                        customer.Birthday = datBirthday;
                        customer.Address = address;
                        customer.MobileNumber = mobileNumber;
                        customer.Email = email;
                        dbContext.ChangeNotify(customer);
                    }
                    Registry registry = dbContext.RegistryRepository.FirstOrDefault(x => x.CompanyId == company.Id && x.CustomerId == customer.Id);
                    if (registry != null) {
                        if (registry.Status == StaticStatus.Pending) throw new Exception("Existe una solicitud de inscripción pendiente. No es posible ingresar otra solicitud");
                        if (registry.DeviceId == deviceId) throw new Exception("La identificación suministrada ya se encuentra registrada mediante este dispositivo. No es posible ingresar otra solicitud");
                        registry.DeviceId = deviceId;
                        dbContext.ChangeNotify(registry);
                    }
                    else
                    {
                        registry = new Registry();
                        registry.DeviceId = deviceId;
                        registry.CompanyId = branch.CompanyId;
                        registry.Customer = customer;
                        registry.RegisterDate = DateTime.UtcNow.AddHours(company.UtcTimeFactor);
                        registry.Status = StaticStatus.Pending;
                        registry.VisitCount = 0;
                        dbContext.RegistryRepository.Add(registry);
                    }
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void RegistryApproval(int registryId)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Registry registry = dbContext.RegistryRepository.FirstOrDefault(x => x.Id == registryId);
                    if (registry.Status != StaticStatus.Pending) throw new Exception("La solicitud de inscripción ya no está pendiente de aprobación");
                    registry.Status = StaticStatus.Active;
                    dbContext.ChangeNotify(registry);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public string TrackCustomerVisit(string deviceId, string accessCode, int employeeId, int productId, int rating, int customerId)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Branch branch = dbContext.BranchRepository.FirstOrDefault(x => x.AccessCode == accessCode);
                    if (branch == null) throw new Exception("No se logró obtener la información de la sucursal que envia la solicitud.");
                    if (!branch.Active) throw new Exception("La sucursal se encuentra inactiva en el sistema. Consulte con su proveedor del servicio.");
                    Company company = dbContext.CompanyRepository.Find(branch.CompanyId);
                    if (company.ExpiresAt < DateTime.UtcNow.AddHours(company.UtcTimeFactor)) throw new Exception("La empresa se encuentra inhabilitada en el sistema. Consulte con su proveedor del servicio.");
                    Registry registry = dbContext.RegistryRepository.FirstOrDefault(x => x.DeviceId == deviceId && x.CompanyId == branch.CompanyId && x.CustomerId == customerId);
                    if (registry == null) throw new Exception("El dispositivo no ha sido registrado. Por favor proceda con el registro.");
                    int visitNumber = registry.VisitCount + 1;
                    bool willApply = false;
                    if (visitNumber == company.PromotionAt)
                    {
                        registry.VisitCount = 0;
                        willApply = true;
                    }
                    else
                    {
                        registry.VisitCount = visitNumber;
                    }
                    dbContext.ChangeNotify(registry);
                    Activity activity = new Activity();
                    activity.RegistryId = registry.Id;
                    activity.CompanyId = branch.CompanyId;
                    activity.BranchId = branch.Id;
                    activity.EmployeeId = employeeId;
                    activity.ServiceId = productId;
                    activity.Rating = rating;
                    activity.VisitDate = DateTime.UtcNow.AddHours(company.UtcTimeFactor);
                    activity.Applied = willApply;
                    dbContext.ActivityRepository.Add(activity);
                    dbContext.Commit();
                    string response = willApply ? company.PromotionMessage : "";
                    return response;
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<ActivityList> GetVisitorActivityList(int companyId, int branchId, string startDate, string endDate)
        {
            DateTime datStartDate = DateTime.ParseExact(startDate + " 00:00:01", strFormat, provider);
            DateTime datEndDate = DateTime.ParseExact(endDate + " 23:59:59", strFormat, provider);
            List<ActivityList> results = new List<ActivityList>();
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    var list = dbContext.CustomerRepository.Join(dbContext.RegistryRepository, x => x.Id, y => y.CustomerId, (x, y) => new { x, y })
                        .Join(dbContext.ActivityRepository, x => x.y.Id, y => y.RegistryId, (x, y) => new { x, y })
                        .Join(dbContext.ServiceRepository, x => x.y.ServiceId, y => y.Id, (x, y) => new { x, y })
                        .Where(x => x.y.CompanyId == companyId && x.x.y.BranchId == branchId && x.x.y.VisitDate >= datStartDate && x.x.y.VisitDate <= datEndDate)
                        .Select(x => new { Id = x.x.y.Id, Name = x.x.x.x.Name, x.x.y.VisitDate, Service = x.y.Description }).ToList();
                        
                    foreach(var entry in list)
                    {
                        ActivityList item = new ActivityList(entry.Id, entry.Name, entry.VisitDate.ToString(strFormat), entry.Service);
                        results.Add(item);
                    }
                    return results;
                    
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<ActivityResume> GetActivityPerEmployeeList(int companyId, int branchId, string startDate, string endDate)
        {
            DateTime datStartDate = DateTime.ParseExact(startDate + " 00:00:01", strFormat, provider);
            DateTime datEndDate = DateTime.ParseExact(endDate + " 23:59:59", strFormat, provider);
            List<ActivityResume> results = new List<ActivityResume>();
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    var list = dbContext.EmployeeRepository.Join(dbContext.ActivityRepository, x => x.Id, y => y.EmployeeId, (x, y) => new { x, y })
                        .Where(x => x.y.CompanyId == companyId && x.y.BranchId == branchId && x.y.VisitDate >= datStartDate && x.y.VisitDate <= datEndDate)
                        .GroupBy(x => x.x.Name)
                        .Select(g => new { Name = g.Key, Count = g.Count() });
                        
                    foreach(var entry in list)
                    {
                        ActivityResume item = new ActivityResume(entry.Name, entry.Count);
                        results.Add(item);
                    }
                    return results;
                    
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}