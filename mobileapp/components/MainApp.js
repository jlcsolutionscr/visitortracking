import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import { validateSessionState, setRewardMessage, setBranch } from '../store/session/actions'
import { setModalMessage } from '../store/ui/actions'

import { StatusBar, View, Text, BackHandler } from 'react-native'
import Modal, { ScaleAnimation, ModalFooter, ModalButton, ModalTitle } from 'react-native-modals'

import SplashScreen from './custom/SplashScreen'
import OutdatedScreen from './outdated/OutdatedScreen'
import Loader from './custom/Loader'
import StartupScreen from './home/StartupScreen'
import RewardScreen from './tracking/RewardScreen'
import TrackingScreen from './tracking/TrackingScreen'

import styles from './styles'

class MainApp extends Component {
  constructor(props) {
    super(props)
    this.state = {
      splashScreenDone: false
    }
  }

  componentDidMount() {
    this.props.validateSessionState()
  }

  splashScreenOnCompleted() {
    this.setState({ splashScreenDone: true })
  }

  handleBackPress() {
    BackHandler.exitApp()
  }

  handleClosePress() {
    this.props.setRewardMessage('')
    this.props.setBranch(null)
  }

  render() {
    const { rewardMessage, sessionStatus, loaderVisible, branch, message, error } = this.props
    const { splashScreenDone } = this.state
    const rootComponent =
      !splashScreenDone || sessionStatus === 'loading' ? (
        <View />
      ) : sessionStatus === 'outdated' ? (
        <OutdatedScreen messageId={1} handleBackPress={this.handleBackPress} />
      ) : branch === null ? (
        <StartupScreen />
      ) : rewardMessage === '' ? (
        <TrackingScreen />
      ) : (
        <RewardScreen
          handleClosePress={this.handleClosePress.bind(this)}
          branch={branch}
          rewardMessage={rewardMessage}
        />
      )
    const visibility = splashScreenDone && loaderVisible
    const modalVisible = error !== '' || message !== ''
    const modalFooter =
      message !== '' ? (
        <ModalFooter>
          <ModalButton
            bordered
            textStyle={styles.modalButtonText}
            text="OK"
            onPress={() => {
              this.props.setModalMessage('')
            }}
          />
        </ModalFooter>
      ) : (
        <ModalFooter>
          <ModalButton
            bordered
            textStyle={styles.modalButtonText}
            text="Recargar"
            onPress={() => {
              this.props.validateSessionState()
            }}
          />
          <ModalButton
            bordered
            textStyle={styles.modalButtonText}
            text="Salir"
            onPress={() => {
              this.handleBackPress()
            }}
          />
        </ModalFooter>
      )
    return (
      <View style={styles.container}>
        <StatusBar hidden={!splashScreenDone} />
        {!splashScreenDone && <SplashScreen onCompleted={this.splashScreenOnCompleted.bind(this)} />}
        {rootComponent}
        <Modal
          modalAnimation={new ScaleAnimation()}
          visible={modalVisible}
          modalStyle={styles.modal}
          modalTitle={<ModalTitle title="JLC Solutions CR" />}
          footer={modalFooter}>
          <View style={styles.dialogContentView}>
            <Text>{message !== '' ? message : error}</Text>
          </View>
        </Modal>
        <Loader visible={visibility} />
      </View>
    )
  }
}

const mapStateToProps = state => {
  return {
    sessionStatus: state.session.sessionStatus,
    loaderVisible: state.ui.loaderVisible,
    branch: state.session.branch,
    rewardMessage: state.session.rewardMessage,
    message: state.ui.message,
    error: state.ui.error
  }
}

const mapDispatchToProps = dispatch => {
  return bindActionCreators(
    {
      validateSessionState,
      setModalMessage,
      setRewardMessage,
      setBranch
    },
    dispatch
  )
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(MainApp)
