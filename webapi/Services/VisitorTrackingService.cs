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

        public List<Customer> GetCustomerList()
        {
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    List<Customer> list = dbContext.CustomerRepository.ToList();
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

        public int getVisitorActivityResume(int companyId, int branchId, string startDate, string endDate)
        {
            DateTime datStartDate = DateTime.ParseExact(startDate + " 00:00:01", strFormat, provider);
            DateTime datEndDate = DateTime.ParseExact(endDate + " 23:59:59", strFormat, provider);
            int visitCount = 0;
            using (var dbContext = new VisitorTrackingContext(_settings.ConnectionString))
            {
                try
                {
                    visitCount = dbContext.ActivityRepository.Where(x => x.CompanyId == companyId && x.BranchId == branchId && x.VisitDate >= datStartDate && x.VisitDate <= datEndDate).Count();
                    return visitCount;
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