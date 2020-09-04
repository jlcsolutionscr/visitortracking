import { START_LOADER, STOP_LOADER, SET_SCANNER_ACTIVE, SET_MODAL_MESSAGE, SET_MODAL_ERROR } from './types'

const configReducer = (state = {}, { type, payload }) => {
  switch (type) {
    case START_LOADER:
      return { ...state, loaderVisible: true }
    case STOP_LOADER:
      return { ...state, loaderVisible: false }
    case SET_SCANNER_ACTIVE:
      return { ...state, scannerActive: payload.active }
    case SET_MODAL_MESSAGE:
      return { ...state, message: payload.message }
    case SET_MODAL_ERROR:
      return { ...state, error: payload.error }
    default:
      return state
  }
}

export default configReducer
