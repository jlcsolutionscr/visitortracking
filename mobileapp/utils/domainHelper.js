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

export async function GetActiveEmployeeList(serviceURL, companyId, token) {
  try {
    const data = '{"MethodName": "GetActiveEmployeeList", "Parameters": {"CompanyId": ' + companyId + '}}'
    const response = await postWithResponse(serviceURL + '/messagewithresponse', token, data)
    if (response === null) return []
    return response
  } catch (e) {
    throw e.message
  }
}

export async function GetActiveServiceList(serviceURL, companyId, token) {
  try {
    const data = '{"MethodName": "GetActiveServiceList", "Parameters": {"CompanyId": ' + companyId + '}}'
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
    const data = '{"MethodName": "CustomerRegistry", "Parameters": {"DeviceId": "' + deviceId + '", "AccessCode": "' + accessCode + '", "Name": "' + customer.Name + '", "Identifier": "' + customer.Identifier + '", "Birthday": "' + customer.Birthday + '", "Address": "' + customer.Address + '", "MobileNumber": "' + customer.MobileNumber + '", "Email": "' + customer.Email + '"}}'
    await post(serviceURL + '/messagenoresponse', token, data)
  } catch (e) {
    throw e.message
  }
}

export async function visitorActivityTracking(serviceURL, deviceId, accessCode, employeeId, serviceId, rating, customerId,  token) {
  try {
    const data = '{"MethodName": "TrackCustomerVisit", "Parameters": {"DeviceId": "' + deviceId + '", "AccessCode": "' + accessCode + '", "EmployeeId": ' + employeeId + ', "ServiceId": ' + serviceId + ', "Rating": ' + rating + ', "CustomerId": ' + customerId + '}}'
    const response = await postWithResponse(serviceURL + '/messagewithresponse', token, data)
    return response
  } catch (e) {
    throw e.message
  }
}
