using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using jlcsolutionscr.com.visitortracking.webapi.customclasses;
using jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain;
using jlcsolutionscr.com.visitortracking.webapi.services;

namespace jlcsolutionscr.com.visitortracking.webapi.controllers
{
    [ApiController]
    [Route("/webservice/")]
    public class VisitorTrackingController : ControllerBase
    {
        private readonly ILogger<VisitorTrackingController> _logger;
        private readonly AppSettings _settings;

        private int companyId;
        private string companyIdentifier;
        private int branchId;
        private int userId;
        private int employeeId;
        private int customerId;
        private string deviceId;
        private string accessCode;
        private Company company;
        private Branch branch;
        private User user;
        private Employee employee;
        private Customer customer;

        public VisitorTrackingController(ILogger<VisitorTrackingController> logger, AppSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        [HttpGet("userlogin")]
        public string UserLogin(string username, string password, string identifier)
        {
            using (var service = new VisitorTrackingService(_settings))
            {
                try
                {
                    Session session = service.UserLogin(username, password, identifier);
                    return JsonSerializer.Serialize(session, new JsonSerializerOptions());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw ex;
                }
            }
        }

        [HttpGet("getlatestappversion")]
        public string GetLatestAppVersion()
        {
            using (var service = new VisitorTrackingService(_settings))
            {
                try
                {
                    string version = service.GetLatestAppVersion();
                    return JsonSerializer.Serialize(version, new JsonSerializerOptions());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw ex;
                }
            }
        }

        [HttpGet("removeinvalidentries")]
        public void RemoveInvalidEntries()
        {
            using (var service = new VisitorTrackingService(_settings))
            {
                try
                {
                    service.RemoveInvalidEntries();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw ex;
                }
            }
        }

        [HttpPost("messagewithresponse")]
        public string MessageWithResponse([FromBody] string text)
        {
            string token = Request.Headers["Authorization"];
            if (token == null) throw new Exception("La solicitud requiere del encabezado de autorización. Reinicie su sesión.");
            token = token.Substring(7);
            MessageData message;
            string response = "";
            try
            {
                message = JsonSerializer.Deserialize<MessageData>(text);
                if (message.MethodName == "" && message.Entity == null && message.Parameters.Count == 0)
                    throw new Exception("El mensaje no contiene la información suficiente para ser procesado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
            using (var service = new VisitorTrackingService(_settings))
            {
                try
                {
                    service.ValidateAccessCode(token);
                    switch (message.MethodName)
                    {
                        case "GetCompanyList":
                            List<IdDescList> companyList = service.GetCompanyList();
                            if (companyList.Count > 0)
                                response = JsonSerializer.Serialize(companyList, new JsonSerializerOptions());
                            break;
                        case "GetCompany":
                            companyId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            company = service.GetCompany(companyId);
                            response = JsonSerializer.Serialize(company, new JsonSerializerOptions());
                            break;
                        case "GetBranchList":
                            companyId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            List<IdDescList> branchList = service.GetBranchList(companyId);
                            if (branchList.Count > 0)
                                response = JsonSerializer.Serialize(branchList, new JsonSerializerOptions());
                            break;
                        case "GetBranch":
                            companyId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            branchId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "BranchId").Value.ToString());
                            branch = service.GetBranch(companyId, branchId);
                            response = JsonSerializer.Serialize(branch, new JsonSerializerOptions());
                            break;
                        case "GetUserList":
                            companyIdentifier = message.Parameters.FirstOrDefault(x => x.Key == "CompanyIdentifier").Value.ToString();
                            List<IdDescList> userList = service.GetUserList(companyIdentifier);
                            if (userList.Count > 0)
                                response = JsonSerializer.Serialize(userList, new JsonSerializerOptions());
                            break;
                        case "GetUser":
                            userId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "UserId").Value.ToString());
                            user = service.GetUser(userId);
                            response = JsonSerializer.Serialize(user, new JsonSerializerOptions());
                            break;
                        case "GetRoleList":
                            List<IdDescList> roleList = service.GetRoleList();
                            if (roleList.Count > 0)
                                response = JsonSerializer.Serialize(roleList, new JsonSerializerOptions());
                            break;
                        case "GetEmployeeList":
                            companyId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            List<IdDescList> employeeList = service.GetEmployeeList(companyId);
                            if (employeeList.Count > 0)
                                response = JsonSerializer.Serialize(employeeList, new JsonSerializerOptions());
                            break;
                        case "GetEmployee":
                            employeeId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "EmployeeId").Value.ToString());
                            employee = service.GetEmployee(employeeId);
                            response = JsonSerializer.Serialize(employee, new JsonSerializerOptions());
                            break;
                        case "GetBranchByCode":
                            accessCode = message.Parameters.FirstOrDefault(x => x.Key == "AccessCode").Value.ToString();
                            branch = service.GetBranchByCode(accessCode);
                            response = JsonSerializer.Serialize(branch, new JsonSerializerOptions());
                            break;
                        case "GetPendingRegistryList":
                            companyId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            List<RegistryData> registryList = service.GetPendingRegistryList(companyId);
                            if (registryList.Count > 0)
                                response = JsonSerializer.Serialize(registryList, new JsonSerializerOptions());
                            break;
                        case "GetRegisteredCustomerList":
                            deviceId = message.Parameters.FirstOrDefault(x => x.Key == "DeviceId").Value.ToString();
                            accessCode = message.Parameters.FirstOrDefault(x => x.Key == "AccessCode").Value.ToString();
                            List<IdDescList> customerList = service.GetRegisteredCustomerList(deviceId, accessCode);
                            if (customerList.Count > 0)
                                response = JsonSerializer.Serialize(customerList, new JsonSerializerOptions());
                            break;
                        case "GetVisitorActivityList":
                            companyId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            branchId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "BranchId").Value.ToString());
                            string startDate = message.Parameters.FirstOrDefault(x => x.Key == "StartDate").Value.ToString();
                            string endDate = message.Parameters.FirstOrDefault(x => x.Key == "EndDate").Value.ToString();
                            List<RegistryData> activityList = service.GetVisitorActivityList(companyId, branchId, startDate, endDate);
                            if (activityList.Count > 0)
                                response = JsonSerializer.Serialize(activityList, new JsonSerializerOptions());
                            break;
                        default:
                            throw new Exception("El método solicitado no ha sido implementado: " + message.MethodName);
                    }
                    return response;
                }
                catch (Exception ex)
                {
                    string strMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _logger.LogError(strMessage);
                    throw new ArgumentException(strMessage);
                }
            }
        }

        [HttpPost("messagenoresponse")]
        public void MessageNoResponse([FromBody] string text)
        {
            string token = Request.Headers["Authorization"];
            if (token == null) throw new Exception("La solicitud requiere del encabezado de autorización. Reinicie su sesión.");
            token = token.Substring(7);
            MessageData message;
            try
            {
                message = JsonSerializer.Deserialize<MessageData>(text);
                if (message.MethodName == "" && message.Entity == null && message.Parameters.Count == 0)
                    throw new Exception("El mensaje no contiene la información suficiente para ser procesado.");
            }
            catch (Exception ex)
            {
                string strMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(strMessage);
                throw ex.InnerException;
            }
            using (var service = new VisitorTrackingService(_settings))
            {
                try
                {
                    service.ValidateAccessCode(token);
                    switch (message.MethodName)
                    {
                        case "AddCompany":
                            company = JsonSerializer.Deserialize<Company>(message.Entity.ToString());
                            service.AddCompany(company);
                            break;
                        case "UpdateCompany":
                            company = JsonSerializer.Deserialize<Company>(message.Entity.ToString());
                            service.UpdateCompany(company);
                            break;
                        case "AddBranch":
                            branch = JsonSerializer.Deserialize<Branch>(message.Entity.ToString());
                            service.AddBranch(branch);
                            break;
                        case "UpdateBranch":
                            branch = JsonSerializer.Deserialize<Branch>(message.Entity.ToString());
                            service.UpdateBranch(branch);
                            break;
                        case "AddUser":
                            user = JsonSerializer.Deserialize<User>(message.Entity.ToString());
                            service.AddUser(user);
                            break;
                        case "UpdateUser":
                            user = JsonSerializer.Deserialize<User>(message.Entity.ToString());
                            service.UpdateUser(user);
                            break;
                        case "AddCustomer":
                            customer = JsonSerializer.Deserialize<Customer>(message.Entity.ToString());
                            service.AddCustomer(customer);
                            break;
                        case "UpdateCustomer":
                            customer = JsonSerializer.Deserialize<Customer>(message.Entity.ToString());
                            service.UpdateCustomer(customer);
                            break;
                        case "AddEmployee":
                            employee = JsonSerializer.Deserialize<Employee>(message.Entity.ToString());
                            service.AddEmployee(employee);
                            break;
                        case "UpdateEmployee":
                            employee = JsonSerializer.Deserialize<Employee>(message.Entity.ToString());
                            service.UpdateEmployee(employee);
                            break;
                        case "CustomerRegistry":
                            deviceId = message.Parameters.FirstOrDefault(x => x.Key == "DeviceId").Value.ToString();
                            accessCode = message.Parameters.FirstOrDefault(x => x.Key == "AccessCode").Value.ToString();
                            int employeeId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "EmployeeId").Value.ToString());
                            string name = message.Parameters.FirstOrDefault(x => x.Key == "Name").Value.ToString();
                            string identifier = message.Parameters.FirstOrDefault(x => x.Key == "Identifier").Value.ToString();
                            string address = message.Parameters.FirstOrDefault(x => x.Key == "Address").Value.ToString();
                            string phoneNumber = message.Parameters.FirstOrDefault(x => x.Key == "PhoneNumber").Value.ToString();
                            string mobileNumber = message.Parameters.FirstOrDefault(x => x.Key == "MobileNumber").Value.ToString();
                            string email = message.Parameters.FirstOrDefault(x => x.Key == "Email").Value.ToString();
                            service.CustomerRegistry(deviceId, accessCode, employeeId, name, identifier, address, phoneNumber, mobileNumber, email);
                            break;
                        case "RegistryApproval":
                            int registryId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "RegistryId").Value.ToString());
                            service.RegistryApproval(registryId);
                            break;
                        case "TrackCustomerVisit":
                            deviceId = message.Parameters.FirstOrDefault(x => x.Key == "DeviceId").Value.ToString();
                            customerId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CustomerId").Value.ToString());
                            accessCode = message.Parameters.FirstOrDefault(x => x.Key == "AccessCode").Value.ToString();
                            employeeId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "EmployeeId").Value.ToString());
                            service.TrackCustomerVisit(deviceId, customerId, employeeId, accessCode);
                            break;
                        default:
                            throw new Exception("El método solicitado no ha sido implementado: " + message.MethodName);
                    }
                }
                catch (Exception ex)
                {
                    string strMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _logger.LogError(strMessage);
                    throw new ArgumentException(strMessage);
                }
            }
        }

        [Route("/error")]
        public string Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return context.Error.Message;
        }
    }
}
