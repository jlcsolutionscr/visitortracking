import Config from 'react-native-config'

import {
  SET_SESSION_TOKEN,
  SET_SESSION_STATUS,
  SET_BRANCH,
  SET_EMPLOYEE_LIST,
  SET_CUSTOMER_LIST,
  SET_ERROR
} from './types'

import {
  getLatestAppVersion,
  userLogin,
  getBranchInfo,
  getEmployeeList,
  getRegisteredCustomerList,
  customerRegistration,
  visitorActivityTracking
} from '../../utils/domainHelper'

import { startLoader, stopLoader, setScannerActive, setModalMessage, setModalError } from '../ui/actions'

import DeviceInfo from 'react-native-device-info'

export const setSessionToken = (token) => {
  return {
    type: SET_SESSION_TOKEN,
    payload: { token }
  }
}

export const setSessionStatus = (deviceId, status) => {
  return {
    type: SET_SESSION_STATUS,
    payload: { deviceId, status }
  }
}

export const setBranch = (entity) => {
  return {
    type: SET_BRANCH,
    payload: { entity }
  }
}

export const setEmployeeList = (list) => {
  return {
    type: SET_EMPLOYEE_LIST,
    payload: { list }
  }
}

export const setCustomerList = (list) => {
  return {
    type: SET_CUSTOMER_LIST,
    payload: { list }
  }
}

export const setError = (error) => {
  return {
    type: SET_ERROR,
    payload: { error }
  }
}

export function validateSessionState () {
  return async (dispatch, getState) => {
    const serviceURL = Config.SERVER_URL
    const { token } = getState().session
    dispatch(startLoader())
    dispatch(setModalError(''))
    try {
      const deviceId = await DeviceInfo.getAndroidId()
      const latestAppVersion = await getLatestAppVersion(serviceURL)
      const currentVersion = await DeviceInfo.getVersion()
      if (latestAppVersion !== currentVersion) {
        dispatch(setSessionStatus(deviceId, 'outdated'))
      } else {
        if (token === null) {
          const session = await userLogin(serviceURL)
          dispatch(setSessionToken(session.Token))
        }
        dispatch(setSessionStatus(deviceId, 'ready'))
      }
      dispatch(stopLoader())
    } catch (error) {
      dispatch(stopLoader())
      dispatch(setModalError(serviceURL + ': ' + error))
    }
  }
}

export function validateCodeInfo (accessCode) {
  return async (dispatch, getState) => {
    const serviceURL = Config.SERVER_URL
    const { deviceId, token } = getState().session
    dispatch(startLoader())
    dispatch(setScannerActive(false))
    dispatch(setError(''))
    try {
      const branch = await getBranchInfo(serviceURL, accessCode, token)
      if (!branch) {
        dispatch(setError('No se encontró información asociada.'))
      } else {
        const employeeList = await getEmployeeList(serviceURL, branch.CompanyId, token)
        dispatch(setEmployeeList(employeeList))
        dispatch(setBranch(branch))
        const customerList = await getRegisteredCustomerList(serviceURL, deviceId, accessCode, token)
        dispatch(setCustomerList(customerList))
      }
      dispatch(stopLoader())
    } catch (error) {
      dispatch(stopLoader())
      dispatch(setError(error))
    }
  }
}

export function registerCustomer (employeeId, customer) {
  return async (dispatch, getState) => {
    const serviceURL = Config.SERVER_URL
    const { deviceId, branch, token } = getState().session
    dispatch(startLoader())
    dispatch(setError(''))
    try {
      await customerRegistration(serviceURL, deviceId, branch.AccessCode, employeeId, customer, token)
      dispatch(setBranch(null))
      dispatch(setModalMessage('Su solicitud de registro ha sido enviada. Una vez aprobada podrás iniciar el registro de sus visitas'))
      dispatch(stopLoader())
    } catch (error) {
      dispatch(stopLoader())
      dispatch(setError(error))
    }
  }
}

export function trackVisitorActivity (customerId, employeeId) {
  return async (dispatch, getState) => {
    const serviceURL = Config.SERVER_URL
    const { deviceId, branch, token } = getState().session
    dispatch(startLoader())
    dispatch(setError(''))
    try {
      await visitorActivityTracking(serviceURL, deviceId, customerId, branch.AccessCode, employeeId, token)
      dispatch(setBranch(null))
      dispatch(setModalMessage('Tu visita ha sido registrada satisfactoriamente'))
      dispatch(stopLoader())
    } catch (error) {
      dispatch(stopLoader())
      dispatch(setError(error))
    }
  }
}
