import Config from 'react-native-config'
import { getAppstoreAppMetadata } from 'react-native-appstore-version-checker'

import {
  SET_SESSION_TOKEN,
  SET_SESSION_STATUS,
  SET_BRANCH,
  SET_EMPLOYEE_LIST,
  SET_SERVICE_LIST,
  SET_CUSTOMER_LIST,
  SET_REWARD_MESSAGE,
  SET_STARTUP_ERROR,
  SET_TRACKING_ERROR,
  SET_ERROR
} from './types'

import {
  userLogin,
  getBranchInfo,
  GetActiveEmployeeList,
  GetActiveServiceList,
  getRegisteredCustomerList,
  visitorActivityTracking
} from '../../utils/domainHelper'

import { startLoader, stopLoader, setScannerActive, setModalMessage, setModalError } from '../ui/actions'

import DeviceInfo from 'react-native-device-info'

export const setSessionToken = token => {
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

export const setBranch = entity => {
  return {
    type: SET_BRANCH,
    payload: { entity }
  }
}

export const setEmployeeList = list => {
  return {
    type: SET_EMPLOYEE_LIST,
    payload: { list }
  }
}

export const setServiceList = list => {
  return {
    type: SET_SERVICE_LIST,
    payload: { list }
  }
}

export const setCustomerList = list => {
  return {
    type: SET_CUSTOMER_LIST,
    payload: { list }
  }
}

export const setRewardMessage = message => {
  return {
    type: SET_REWARD_MESSAGE,
    payload: { message }
  }
}

export const setStartupError = error => {
  return {
    type: SET_STARTUP_ERROR,
    payload: { error }
  }
}

export const setTrackingError = error => {
  return {
    type: SET_TRACKING_ERROR,
    payload: { error }
  }
}

export const setError = error => {
  return {
    type: SET_ERROR,
    payload: { error }
  }
}

export function validateSessionState() {
  return async (dispatch, getState) => {
    const serviceURL = Config.SERVER_URL
    const { token } = getState().session
    dispatch(startLoader())
    dispatch(setModalError(''))
    try {
      const metadata = await getAppstoreAppMetadata('com.jlcvisitortracking')
      const deviceId = await DeviceInfo.getAndroidId()
      const currentVersion = await DeviceInfo.getVersion()
      if (metadata.version !== currentVersion) {
        dispatch(setSessionStatus(deviceId, 'outdated'))
      } else {
        if (token === null) {
          const session = await userLogin(serviceURL)
          dispatch(setSessionToken(session.Token))
        }
        dispatch(setSessionStatus(deviceId, 'ready'))
        dispatch(validateCodeInfo('6b689301-c594-403d-9185-bb339d254d3c'))
      }
      dispatch(stopLoader())
    } catch (error) {
      dispatch(stopLoader())
      dispatch(setModalError(serviceURL + ': ' + error))
    }
  }
}

export function validateCodeInfo(accessCode) {
  return async (dispatch, getState) => {
    const serviceURL = Config.SERVER_URL
    const { deviceId, token } = getState().session
    dispatch(startLoader())
    dispatch(setScannerActive(false))
    dispatch(setStartupError(''))
    try {
      const branch = await getBranchInfo(serviceURL, accessCode, token)
      if (!branch) {
        dispatch(setStartupError('No se encontró una sucursal asociada con el código QR.'))
      } else {
        dispatch(setBranch(branch))
        const employeeList = await GetActiveEmployeeList(serviceURL, branch.CompanyId, token)
        dispatch(setEmployeeList(employeeList))
        const serviceList = await GetActiveServiceList(serviceURL, branch.CompanyId, token)
        dispatch(setServiceList(serviceList))
        const customerList = await getRegisteredCustomerList(serviceURL, deviceId, accessCode, token)
        dispatch(setCustomerList(customerList))
      }
      dispatch(setTrackingError(''))
      dispatch(stopLoader())
    } catch (error) {
      dispatch(stopLoader())
      dispatch(setStartupError(error))
    }
  }
}

export function trackVisitorActivity(employeeId, serviceId, rating, comment, customerId, identifier) {
  return async (dispatch, getState) => {
    const serviceURL = Config.SERVER_URL
    const { deviceId, branch, token } = getState().session
    dispatch(startLoader())
    dispatch(setTrackingError(''))
    try {
      const response = await visitorActivityTracking(
        serviceURL,
        deviceId,
        branch.AccessCode,
        employeeId,
        serviceId,
        rating,
        comment,
        customerId,
        identifier,
        token
      )
      if (response === '') {
        dispatch(setBranch(null))
        dispatch(setModalMessage('Tu visita ha sido registrada satisfactoriamente'))
      } else {
        dispatch(setRewardMessage(response))
      }
      dispatch(stopLoader())
    } catch (error) {
      dispatch(stopLoader())
      dispatch(setTrackingError(error))
    }
  }
}
