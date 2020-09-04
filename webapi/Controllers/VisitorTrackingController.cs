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
    public class VisitorTrackingController : ControllerBase
    {
        private readonly ILogger<VisitorTrackingController> _logger;
        private readonly AppSettings _settings;

        private int companyEntityId;
        private string companyEntityIdentifier;
        private int branchId;
        private int userId;
        private int employeeId;
        private int serviceId;
        private int customerId;
        private string deviceId;
        private string accessCode;
        private string startDate;
        private string endDate;
        private Company companyEntity;
        private Branch branchEntity;
        private User userEntity;
        private Employee employeeEntity;
        private Service serviceEntity;
        private Customer customerEntity;

        public VisitorTrackingController(ILogger<VisitorTrackingController> logger, AppSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        [HttpGet("status")]
        public string Status(string username, string password, string identifier)
        {
            return "Active";
        }

        [HttpGet("userlogin")]
        public string UserLogin(string username, string password, string identifier)
        {
            using (var service = new VisitorTrackingService(_settings))
            {
                try
                {
                    string fixedPassword = password.Replace(" ", "+");
                    Session session = service.UserLogin(username, fixedPassword, identifier);
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
                            List<IdDescList> companyEntityList = service.GetCompanyList();
                            if (companyEntityList.Count > 0)
                                response = JsonSerializer.Serialize(companyEntityList, new JsonSerializerOptions());
                            break;
                        case "GetCompany":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            companyEntity = service.GetCompany(companyEntityId);
                            response = JsonSerializer.Serialize(companyEntity, new JsonSerializerOptions());
                            break;
                        case "GetBranchList":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            List<IdDescList> branchEntityList = service.GetBranchList(companyEntityId);
                            if (branchEntityList.Count > 0)
                                response = JsonSerializer.Serialize(branchEntityList, new JsonSerializerOptions());
                            break;
                        case "GetBranch":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            branchId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "BranchId").Value.ToString());
                            branchEntity = service.GetBranch(companyEntityId, branchId);
                            response = JsonSerializer.Serialize(branchEntity, new JsonSerializerOptions());
                            break;
                        case "GetUserList":
                            companyEntityIdentifier = message.Parameters.FirstOrDefault(x => x.Key == "CompanyIdentifier").Value.ToString();
                            List<IdDescList> userList = service.GetUserList(companyEntityIdentifier);
                            if (userList.Count > 0)
                                response = JsonSerializer.Serialize(userList, new JsonSerializerOptions());
                            break;
                        case "GetUser":
                            userId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "UserId").Value.ToString());
                            userEntity = service.GetUser(userId);
                            response = JsonSerializer.Serialize(userEntity, new JsonSerializerOptions());
                            break;
                        case "GetRoleList":
                            List<IdDescList> roleList = service.GetRoleList();
                            if (roleList.Count > 0)
                                response = JsonSerializer.Serialize(roleList, new JsonSerializerOptions());
                            break;
                        case "GetEmployeeList":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            List<IdDescList> employeeList = service.GetEmployeeList(companyEntityId);
                            if (employeeList.Count > 0)
                                response = JsonSerializer.Serialize(employeeList, new JsonSerializerOptions());
                            break;
                        case "GetActiveEmployeeList":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            List<IdDescList> activeEmployeeList = service.GetActiveEmployeeList(companyEntityId);
                            if (activeEmployeeList.Count > 0)
                                response = JsonSerializer.Serialize(activeEmployeeList, new JsonSerializerOptions());
                            break;
                        case "GetEmployee":
                            employeeId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "EmployeeId").Value.ToString());
                            employeeEntity = service.GetEmployee(employeeId);
                            response = JsonSerializer.Serialize(employeeEntity, new JsonSerializerOptions());
                            break;
                        case "GetServiceList":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            List<IdDescList> serviceList = service.GetServiceList(companyEntityId);
                            if (serviceList.Count > 0)
                                response = JsonSerializer.Serialize(serviceList, new JsonSerializerOptions());
                            break;
                        case "GetActiveServiceList":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            List<IdDescList> activeServiceList = service.GetActiveServiceList(companyEntityId);
                            if (activeServiceList.Count > 0)
                                response = JsonSerializer.Serialize(activeServiceList, new JsonSerializerOptions());
                            break;
                        case "GetService":
                            serviceId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "ServiceId").Value.ToString());
                            serviceEntity = service.GetService(serviceId);
                            response = JsonSerializer.Serialize(serviceEntity, new JsonSerializerOptions());
                            break;
                        case "GetBranchByCode":
                            accessCode = message.Parameters.FirstOrDefault(x => x.Key == "AccessCode").Value.ToString();
                            branchEntity = service.GetBranchByCode(accessCode);
                            response = JsonSerializer.Serialize(branchEntity, new JsonSerializerOptions());
                            break;
                        case "GetCustomerList":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            List<IdDescList> registryList = service.GetCustomerList(companyEntityId);
                            if (registryList.Count > 0)
                                response = JsonSerializer.Serialize(registryList, new JsonSerializerOptions());
                            break;
                        case "GetCustomer":
                            customerId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CustomerId").Value.ToString());
                            customerEntity = service.GetCustomer(customerId);
                            response = JsonSerializer.Serialize(customerEntity, new JsonSerializerOptions());
                            break;
                        case "GetRegisteredCustomerList":
                            deviceId = message.Parameters.FirstOrDefault(x => x.Key == "DeviceId").Value.ToString();
                            accessCode = message.Parameters.FirstOrDefault(x => x.Key == "AccessCode").Value.ToString();
                            List<IdDescList> customerList = service.GetRegisteredCustomerList(deviceId, accessCode);
                            if (customerList.Count > 0)
                                response = JsonSerializer.Serialize(customerList, new JsonSerializerOptions());
                            break;
                        case "GetVisitorActivityList":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            branchId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "BranchId").Value.ToString());
                            startDate = message.Parameters.FirstOrDefault(x => x.Key == "StartDate").Value.ToString();
                            endDate = message.Parameters.FirstOrDefault(x => x.Key == "EndDate").Value.ToString();
                            List<ActivityList> activityList = service.GetVisitorActivityList(companyEntityId, branchId, startDate, endDate);
                            if (activityList.Count > 0)
                                response = JsonSerializer.Serialize(activityList, new JsonSerializerOptions());
                            break;
                        case "GetActivityPerEmployeeList":
                            companyEntityId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            branchId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "BranchId").Value.ToString());
                            startDate = message.Parameters.FirstOrDefault(x => x.Key == "StartDate").Value.ToString();
                            endDate = message.Parameters.FirstOrDefault(x => x.Key == "EndDate").Value.ToString();
                            List<ActivityResume> activityResumeList = service.GetActivityPerEmployeeList(companyEntityId, branchId, startDate, endDate);
                            if (activityResumeList.Count > 0)
                                response = JsonSerializer.Serialize(activityResumeList, new JsonSerializerOptions());
                            break;
                        case "TrackCustomerVisit":
                            deviceId = message.Parameters.FirstOrDefault(x => x.Key == "DeviceId").Value.ToString();
                            accessCode = message.Parameters.FirstOrDefault(x => x.Key == "AccessCode").Value.ToString();
                            employeeId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "EmployeeId").Value.ToString());
                            serviceId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "ServiceId").Value.ToString());
                            int rating = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "Rating").Value.ToString());
                            string comment = message.Parameters.FirstOrDefault(x => x.Key == "Comment").Value.ToString();
                            customerId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CustomerId").Value.ToString());
                            string identifier = message.Parameters.FirstOrDefault(x => x.Key == "Identifier").Value.ToString();
                            string promotionMessage = service.TrackCustomerVisit(deviceId, accessCode, employeeId, serviceId, rating, comment, customerId, identifier);
                            response = JsonSerializer.Serialize(promotionMessage, new JsonSerializerOptions());
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
                            companyEntity = JsonSerializer.Deserialize<Company>(message.Entity.ToString());
                            service.AddCompany(companyEntity);
                            break;
                        case "UpdateCompany":
                            companyEntity = JsonSerializer.Deserialize<Company>(message.Entity.ToString());
                            service.UpdateCompany(companyEntity);
                            break;
                        case "AddBranch":
                            branchEntity = JsonSerializer.Deserialize<Branch>(message.Entity.ToString());
                            service.AddBranch(branchEntity);
                            break;
                        case "UpdateBranch":
                            branchEntity = JsonSerializer.Deserialize<Branch>(message.Entity.ToString());
                            service.UpdateBranch(branchEntity);
                            break;
                        case "AddUser":
                            userEntity = JsonSerializer.Deserialize<User>(message.Entity.ToString());
                            service.AddUser(userEntity);
                            break;
                        case "UpdateUser":
                            userEntity = JsonSerializer.Deserialize<User>(message.Entity.ToString());
                            service.UpdateUser(userEntity);
                            break;
                        case "AddCustomer":
                            customerEntity = JsonSerializer.Deserialize<Customer>(message.Entity.ToString());
                            service.AddCustomer(customerEntity);
                            break;
                        case "UpdateCustomer":
                            customerEntity = JsonSerializer.Deserialize<Customer>(message.Entity.ToString());
                            service.UpdateCustomer(customerEntity);
                            break;
                        case "AddEmployee":
                            employeeEntity = JsonSerializer.Deserialize<Employee>(message.Entity.ToString());
                            service.AddEmployee(employeeEntity);
                            break;
                        case "UpdateEmployee":
                            employeeEntity = JsonSerializer.Deserialize<Employee>(message.Entity.ToString());
                            service.UpdateEmployee(employeeEntity);
                            break;
                        case "AddService":
                            serviceEntity = JsonSerializer.Deserialize<Service>(message.Entity.ToString());
                            service.AddService(serviceEntity);
                            break;
                        case "UpdateService":
                            serviceEntity = JsonSerializer.Deserialize<Service>(message.Entity.ToString());
                            service.UpdateService(serviceEntity);
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
