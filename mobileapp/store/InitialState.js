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
    notInList: true,
    identifier: '',
    selectedCustomerId: 0,
    selectedEmployeeId: 0,
    selectedServiceId: 0,
    rating: 0,
    comment: '',
    rewardMessage: '',
    error: ''
  }
}
