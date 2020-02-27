import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import { setScannerActive } from '../../store/ui/actions'

import { validateCodeInfo } from '../../store/session/actions'
import { View, Text } from 'react-native'

import { RNCamera } from 'react-native-camera'
import Button from '../custom/Button'

import styles from '../styles'
  
class StartupScreen extends Component {
  render () {
    const { error, scannerActive } = this.props
    return <View key='1' style={styles.subContainer}>
      {scannerActive ? <RNCamera
        ref={ref => {
          this.camera = ref;
        }}
        style={{
          flex: 1,
          width: '100%'
        }}
        captureAudio={false}
        onGoogleVisionBarcodesDetected={(obj) => this.props.validateCodeInfo(obj.barcodes[0].data)}
      /> :
      <View>
        <View style={styles.header}>
          <Text style={styles.title}>REGISTRO DE VISITAS</Text>
        </View>
        <Text style={styles.specialText}>Precione el botón para continuar</Text>
        <Button
          title="Escanear"
          titleUpperCase
          containerStyle={{marginTop: 20}} 
          onPress={() => this.props.setScannerActive(true)}
        />
        {error !== '' && <Text style={styles.errorText}>{error}</Text>}
      </View>}
    </View>
  }
}

const mapStateToProps = (state) => {
  return {
    scannerActive: state.ui.scannerActive,
    error: state.session.error
  }
}

const mapDispatchToProps = (dispatch) => {
  return bindActionCreators({
    setScannerActive,
    validateCodeInfo
  }, dispatch)
}

export default connect(mapStateToProps, mapDispatchToProps)(StartupScreen)
