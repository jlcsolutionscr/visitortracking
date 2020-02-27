import {
  START_LOADER,
  STOP_LOADER,
  SET_SCANNER_ACTIVE,
  SET_MODAL_MESSAGE,
  SET_MODAL_ERROR
} from './types'

export const startLoader = () => {
  return {
    type: START_LOADER
  }
}

export const stopLoader = () => {
  return {
    type: STOP_LOADER
  }
}

export const setScannerActive = (active) => {
  return {
    type: SET_SCANNER_ACTIVE,
    payload: { active }
  }
}

export const setModalMessage = (message) => {
  return {
    type: SET_MODAL_MESSAGE,
    payload: { message }
  }
}

export const setModalError = (error) => {
  return {
    type: SET_MODAL_ERROR,
    payload: { error }
  }
}
