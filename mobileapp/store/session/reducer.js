import {
  SET_SESSION_TOKEN,
  SET_SESSION_STATUS,
  SET_BRANCH,
  SET_EMPLOYEE_LIST,
  SET_SERVICE_LIST,
  SET_CUSTOMER_LIST,
  SET_REWARD_MESSAGE,
  SET_NOT_IN_LIST,
  SET_IDENTIFIER,
  SET_SELECTED_CUSTOMER,
  SET_SELECTED_EMPLOYEE,
  SET_SELECTED_SERVICE,
  SET_RATING,
  SET_COMMENT,
  SET_STARTUP_ERROR,
  SET_TRACKING_ERROR,
  SET_ERROR
} from './types'

export const configReducer = (state = {}, { type, payload }) => {
  switch (type) {
    case SET_SESSION_TOKEN:
      return { ...state, token: payload.token }
    case SET_SESSION_STATUS:
      return {
        ...state,
        deviceId: payload.deviceId,
        sessionStatus: payload.status
      }
    case SET_BRANCH:
      return { ...state, branch: payload.entity }
    case SET_EMPLOYEE_LIST:
      return { ...state, employeeList: payload.list }
    case SET_SERVICE_LIST:
      return { ...state, serviceList: payload.list }
    case SET_CUSTOMER_LIST:
      return { ...state, customerList: payload.list }
    case SET_CUSTOMER_LIST:
      return { ...state, customerList: payload.list }
    case SET_NOT_IN_LIST:
      return { ...state, notInList: payload.value }
    case SET_IDENTIFIER:
      return { ...state, identifier: payload.value }
    case SET_SELECTED_CUSTOMER:
      return { ...state, selectedCustomerId: payload.id }
    case SET_SELECTED_EMPLOYEE:
      return { ...state, selectedEmployeeId: payload.id }
    case SET_SELECTED_SERVICE:
      return { ...state, selectedServiceId: payload.id }
    case SET_RATING:
      return { ...state, rating: payload.value }
    case SET_COMMENT:
      return { ...state, comment: payload.value }
    case SET_REWARD_MESSAGE:
      return { ...state, rewardMessage: payload.message }
    case SET_STARTUP_ERROR:
      return { ...state, startupError: payload.error }
    case SET_TRACKING_ERROR:
      return { ...state, trackingError: payload.error }
    case SET_ERROR:
      return { ...state, error: payload.error }
    default:
      return state
  }
}

export default configReducer
