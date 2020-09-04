export const INITIAL_STATE = {
  ui: {
    loading: true,
    loaderVisible: false,
    splashScreenDone: false,
    scannerActive: false,
    message: '',
    startupError: '',
    trackingError: '',
    error: ''
  },
  session: {
    sessionStatus: 'loading',
    token: null,
    deviceId: null,
    branch: null,
    employeeList: [],
    customerList: [],
    serviceList: [],
    rewardMessage: '',
    error: ''
  }
}
