import React from 'react'

import { Dimensions, StyleSheet, View, Text, Linking } from 'react-native'
import Button from '../custom/Button'

const { height } = Dimensions.get('window')
const remY = height / 683.4285714285714

const OutdatedScreen = (props) => {
  return (
    <View style={styles.content}>
      <Text style={styles.text}>
        La aplicación se encuentra desactualizada. Por favor ingrese al Google Play Store del dispositivo y proceda con la actualización.
      </Text>
      <Button
        containerStyle={styles.buttonContainer}
        style={styles.button}
        titleUpperCase
        title='Cerrar App'
        onPress={() => props.handleBackPress()}
      />
      <Button
        containerStyle={styles.buttonContainer}
        style={styles.button}
        titleUpperCase
        title='Ir al Play Store'
        onPress={() => Linking.openURL('market://details?id=googoo.android.btgps')}
      />
    </View>
  )
}

const styles = StyleSheet.create({
  content: {
    flex: 1, 
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#08415C'
  },
  text: {
    color: 'white',
    textAlign: 'center',
    fontSize: 22,
    marginBottom: 20
  },
  buttonContainer: {
    padding: 0,
    marginBottom: 2
  },
  button: {
    backgroundColor: '#909596',
    borderColor: '#909596',
    borderRadius: 2,
    paddingLeft: (60 * remY),
    paddingRight: (60 * remY),
    height: (40 * remY)
  }
})

export default OutdatedScreen
