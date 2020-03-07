import { getWithResponse, post, postWithResponse } from "../utils/requestHelper"

export async function getLatestAppVersion(serviceURL) {
  try {
    const endpoint = serviceURL + '/getlatestappversion'
    const response = await getWithResponse(endpoint)
    return response
  } catch (e) {
    throw e.message
  }
}

export async function userLogin(serviceURL) {
  try {
    const endpoint = serviceURL + '/userlogin?username=MOBILEAPP&password=/SOHlDytfCDqqGitmLZJgw==&identifier=1'
    const response = await getWithResponse(endpoint)
    return response
  } catch (e) {
    throw e.message
  }
}

export async function getBranchInfo(serviceURL, accessCode, token) {
  try {
    const data = '{"MethodName": "GetBranchByCode", "Parameters": {"AccessCode": "' + accessCode + '"}}'
    const response = await postWithResponse(serviceURL + '/messagewithresponse', token, data)
    return response
  } catch (e) {
    throw e.message
  }
}

export async function getEmployeeList(serviceURL, companyId, token) {
  try {
    const data = '{"MethodName": "GetEmployeeList", "Parameters": {"CompanyId": ' + companyId + '}}'
    const response = await postWithResponse(serviceURL + '/messagewithresponse', token, data)
    if (response === null) return []
    return response
  } catch (e) {
    throw e.message
  }
}

export async function getRegisteredCustomerList(serviceURL, deviceId, accessCode, token) {
  try {
    const data = '{"MethodName": "GetRegisteredCustomerList", "Parameters": {"DeviceId": "' + deviceId + '", "AccessCode": "' + accessCode + '"}}'
    const response = await postWithResponse(serviceURL + '/messagewithresponse', token, data)
    if (response === null) return []
    return response
  } catch (e) {
    throw e.message
  }
}

export async function customerRegistration(serviceURL, deviceId, accessCode, customer, token) {
  try {
    const data = '{"MethodName": "CustomerRegistry", "Parameters": {"DeviceId": "' + deviceId + '", "AccessCode": "' + accessCode + '", "Name": "' + customer.Name + '", "Identifier": "' + customer.Identifier + '", "Birthday": "' + + ', "Address": "' + customer.Address + '", "MobileNumber": "' + customer.MobileNumber + '", "Email": "' + customer.Email + '"}}'
    await post(serviceURL + '/messagenoresponse', token, data)
  } catch (e) {
    throw e.message
  }
}

export async function visitorActivityTracking(serviceURL, deviceId, customerId, accessCode, employeeId, token) {
  try {
    const data = '{"MethodName": "TrackCustomerVisit", "Parameters": {"DeviceId": "' + deviceId + '", "CustomerId": ' + customerId + ', "AccessCode": "' + accessCode + '", "EmployeeId": ' + employeeId + '}}'
    await post(serviceURL + '/messagenoresponse', token, data)
  } catch (e) {
    throw e.message
  }
}
