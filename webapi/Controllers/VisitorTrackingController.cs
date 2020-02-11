using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using jlcsolutionscr.com.visitortracking.webapi.customclasses;
using jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain;
using jlcsolutionscr.com.visitortracking.webapi.services;

namespace jlcsolutionscr.com.visitortracking.webapi.controllers
{
    [ApiController]
    [Route("/visitortracking/")]
    public class VisitorTrackingController : ControllerBase
    {
        private readonly ILogger<VisitorTrackingController> _logger;
        private readonly AppSettings _settings;

        private int companyId;
        private int branchId;
        private int employeeId;
        private int customerId;
        private string deviceId;
        private string accessCode;
        private Company company;
        private Branch branch;
        private Employee employee;
        private Customer customer;

        public VisitorTrackingController(ILogger<VisitorTrackingController> logger, AppSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        [HttpPost("messagewithresponse")]
        public string MessageWithResponse([FromBody] string text)
        {
            MessageData message;
            string response = "";
            try
            {
                message = JsonSerializer.Deserialize<MessageData>(text);
                if (message.MethodName == "" && message.Entity == "" && message.Parameters.Count == 0)
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
                    switch (message.MethodName)
                    {
                        case "GetCompanyList":
                            IList<Company> companyList = service.GetCompanyList();
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
                            IList<Branch> branchList = service.GetBranchList(companyId);
                            if (branchList.Count > 0)
                                response = JsonSerializer.Serialize(branchList, new JsonSerializerOptions());
                            break;
                        case "GetBranch":
                            branchId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "BranchId").Value.ToString());
                            branch = service.GetBranch(branchId);
                            response = JsonSerializer.Serialize(branch, new JsonSerializerOptions());
                            break;
                        case "GetCustomer":
                            customerId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CustomerId").Value.ToString());
                            customer = service.GetCustomer(customerId);
                            response = JsonSerializer.Serialize(customer, new JsonSerializerOptions());
                            break;
                        case "GetEmployeeList":
                            companyId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            IList<Employee> employeeList = service.GetEmployeeList(companyId);
                            if (employeeList.Count > 0)
                                response = JsonSerializer.Serialize(employeeList, new JsonSerializerOptions());
                            break;
                        case "GetEmployee":
                            employeeId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "EmployeeId").Value.ToString());
                            employee = service.GetEmployee(employeeId);
                            response = JsonSerializer.Serialize(employee, new JsonSerializerOptions());
                            break;
                        default:
                            throw new Exception("El método solicitado no ha sido implementado: " + message.MethodName);
                    }
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw ex;
                }
            }
        }

        [HttpPost("messagenoresponse")]
        public void MessageNoResponse([FromBody] string text)
        {
            MessageData message;
            string entity = "";
            try
            {
                message = JsonSerializer.Deserialize<MessageData>(text);
                if (message.MethodName == "" && message.Entity == "" && message.Parameters.Count == 0)
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
                    switch (message.MethodName)
                    {
                        case "AddCompany":
                            entity = message.Entity.Replace("'", "\"");
                            company = JsonSerializer.Deserialize<Company>(entity);
                            service.AddCompany(company);
                            break;
                        case "UpdateCompany":
                            entity = message.Entity.Replace("'", "\"");
                            company = JsonSerializer.Deserialize<Company>(entity);
                            service.UpdateCompany(company);
                            break;
                        case "AddBranch":
                            entity = message.Entity.Replace("'", "\"");
                            branch = JsonSerializer.Deserialize<Branch>(entity);
                            service.AddBranch(branch);
                            break;
                        case "UpdateBranch":
                            entity = message.Entity.Replace("'", "\"");
                            branch = JsonSerializer.Deserialize<Branch>(entity);
                            service.UpdateBranch(branch);
                            break;
                        case "AddCustomer":
                            entity = message.Entity.Replace("'", "\"");
                            customer = JsonSerializer.Deserialize<Customer>(entity);
                            service.AddCustomer(customer);
                            break;
                        case "UpdateCustomer":
                            entity = message.Entity.Replace("'", "\"");
                            customer = JsonSerializer.Deserialize<Customer>(entity);
                            service.UpdateCustomer(customer);
                            break;
                        case "AddEmployee":
                            entity = message.Entity.Replace("'", "\"");
                            employee = JsonSerializer.Deserialize<Employee>(entity);
                            service.AddEmployee(employee);
                            break;
                        case "UpdateEmployee":
                            entity = message.Entity.Replace("'", "\"");
                            employee = JsonSerializer.Deserialize<Employee>(entity);
                            service.UpdateEmployee(employee);
                            break;
                        case "CustomerRegistry":
                            deviceId = message.Parameters.FirstOrDefault(x => x.Key == "DeviceId").Value.ToString();
                            accessCode = message.Parameters.FirstOrDefault(x => x.Key == "AccessCode").Value.ToString();
                            string name = message.Parameters.FirstOrDefault(x => x.Key == "Name").Value.ToString();
                            string identifier = message.Parameters.FirstOrDefault(x => x.Key == "Identifier").Value.ToString();
                            string address = message.Parameters.FirstOrDefault(x => x.Key == "Address").Value.ToString();
                            string phoneNumber = message.Parameters.FirstOrDefault(x => x.Key == "PhoneNumber").Value.ToString();
                            string mobileNumber = message.Parameters.FirstOrDefault(x => x.Key == "MobileNumber").Value.ToString();
                            string email = message.Parameters.FirstOrDefault(x => x.Key == "Email").Value.ToString();
                            customer = new Customer();
                            customer.Name = name;
                            customer.Identifier = identifier;
                            customer.Address = address;
                            customer.PhoneNumber = phoneNumber;
                            customer.MobileNumber = mobileNumber;
                            customer.Email = email;
                            service.CustomerRegistry(deviceId, accessCode, customer);
                            break;
                        case "ApproveRegistry":
                            deviceId = message.Parameters.FirstOrDefault(x => x.Key == "DeviceId").Value.ToString();
                            companyId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CompanyId").Value.ToString());
                            customerId = int.Parse(message.Parameters.FirstOrDefault(x => x.Key == "CustomerId").Value.ToString());
                            service.ApproveRegistry(deviceId, companyId, customerId);
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
                    _logger.LogError(ex.Message);
                    throw ex;
                }
            }
        }
    }
}
