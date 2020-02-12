using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
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
                        dbContext.NotificarEliminacion(entry);
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

        public User UserLogin(string username, string password, string identifier)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Company company = dbContext.CompanyRepository.FirstOrDefault(x => x.Identifier == identifier);
                    if (company == null) throw new Exception("La identificación suministrada no pertenece a ninguna empresa registrada en el sistema. Por favor verifique la información suministrada.");
                    if (company.ExpiresAt < DateTime.Today) throw new Exception("La vigencia del plan de facturación ha expirado. Por favor, pongase en contacto con su proveedor de servicio.");
                    User user = null;
                    if (username.ToUpper() == "ADMIN" || username.ToUpper() == "MOBILEAPP")
                        user = dbContext.UserRepository.FirstOrDefault(x => x.Username == username.ToUpper());
                    else
                        user = dbContext.UserRepository.FirstOrDefault(x => x.Username == username.ToUpper() && x.Identifier == identifier);
                    if (user == null) throw new Exception("Usuario no registrado en la empresa suministrada. Por favor verifique la información suministrada.");
                    if (user.Password != password) throw new Exception("Los credenciales suministrados no son válidos. Verifique los credenciales suministrados.");
                    string token = GenerateAuthorization();
                    user.Token = token;
                    return user;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<Company> GetCompanyList()
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Company> list = dbContext.CompanyRepository.ToList();
                    return list;
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
                    dbContext.NotificarModificacion(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<User> GetUserList(string identifier)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<User> list = dbContext.UserRepository.Where(x => x.Identifier == identifier).ToList();
                    return list;
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
                    dbContext.UserRepository.Add(entity);
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
                    dbContext.NotificarModificacion(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<Branch> GetBranchList(int companyId)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Branch> list = dbContext.BranchRepository.Where(x => x.CompanyId == companyId).ToList();
                    return list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Branch GetBranch(int id)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Branch entity = dbContext.BranchRepository.FirstOrDefault(x => x.Id == id);
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
                    dbContext.NotificarModificacion(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<Customer> GetCustomerList(string deviceId)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Customer> list = dbContext.CustomerRepository.Join(dbContext.RegistryRepository, x => x.Id, y => y.CustomerId, (x, y) => new { x, y })
                        .Where(x => x.y.DeviceId == deviceId)
                        .Select(x => x.x).ToList();
                    return list;
                }
                catch (Exception ex)
                {
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
                    dbContext.NotificarModificacion(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public List<Employee> GetEmployeeList(int companyId)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Employee> list = dbContext.EmployeeRepository.Where(x => x.CompanyId == companyId).ToList();
                    return list;
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
                    dbContext.NotificarModificacion(entity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void CustomerRegistry(string deviceId, string accessCode, Customer customer)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Branch branch = dbContext.BranchRepository.FirstOrDefault(x => x.AccessCode == accessCode);
                    if (branch == null) throw new Exception("No se logró obtener la información de la sucursal que envia la solicitud.");
                    Company company = dbContext.CompanyRepository.Find(branch.CompanyId);
                    if (company.ExpiresAt < DateTime.Now) throw new Exception("La empresa se encuentra inhabilitada en el sistema. Consulte con su proveedor del servicio.");
                    dbContext.CustomerRepository.Add(customer);
                    Registry registry = new Registry();
                    registry.DeviceId = deviceId;
                    registry.CompanyId = branch.CompanyId;
                    registry.CustomerId = customer.Id;
                    registry.RegisterDate = DateTime.Now;
                    registry.Status = "P";
                    registry.VisitCount = 1;
                    dbContext.RegistryRepository.Add(registry);
                    Activity activity = new Activity();
                    activity.DeviceId = deviceId;
                    activity.CompanyId = branch.CompanyId;
                    activity.CustomerId = customer.Id;
                    activity.BranchId = branch.Id;
                    activity.VisitDate = DateTime.Now;
                    activity.Applied = false;
                    dbContext.ActivityRepository.Add(activity);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public void ApproveRegistry(string deviceId, int companyId, int customerId)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Registry registry = dbContext.RegistryRepository.FirstOrDefault(x => x.DeviceId == deviceId && x.CompanyId == companyId && x.CustomerId == customerId);
                    registry.Status = "A";
                    dbContext.NotificarModificacion(registry);
                    dbContext.Commit();
                }
                catch (Exception ex)
                {
                    dbContext.RollBack();
                    throw ex;
                }
            }
        }

        public string TrackCustomerVisit(string deviceId, int customerId, int employeeId, string accessCode)
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    Branch branch = dbContext.BranchRepository.FirstOrDefault(x => x.AccessCode == accessCode);
                    if (branch == null) throw new Exception("No se logró obtener la información de la sucursal que envia la solicitud.");
                    Company company = dbContext.CompanyRepository.Find(branch.CompanyId);
                    if (company.ExpiresAt < DateTime.Now) throw new Exception("La empresa se encuentra inhabilitada en el sistema. Consulte con su proveedor del servicio.");
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
                    dbContext.NotificarModificacion(registry);
                    Activity activity = new Activity();
                    activity.DeviceId = deviceId;
                    activity.CompanyId = branch.CompanyId;
                    activity.CustomerId = customerId;
                    activity.BranchId = branch.Id;
                    activity.VisitDate = DateTime.Now;
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

        public IList<Activity> getVisitorActivityList(int companyId, int branchId, string startDate, string endDate)
        {
            DateTime datStartDate = DateTime.ParseExact(startDate + " 00:00:01", strFormat, provider);
            DateTime datEndDate = DateTime.ParseExact(endDate + " 23:59:59", strFormat, provider);
            IList<Activity> list = new List<Activity>();
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    list = dbContext.ActivityRepository.Where(x => x.CompanyId == companyId && x.BranchId == branchId && x.VisitDate >= datStartDate && x.VisitDate <= datEndDate).ToList();
                    return list;
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