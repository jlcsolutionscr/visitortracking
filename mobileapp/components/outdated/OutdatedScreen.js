import React from 'react'

import { StyleSheet, View, Text, Linking } from 'react-native'
import Button from '../custom/Button'

const OutdatedScreen = props => {
  return (
    <View style={styles.content}>
      <Text style={styles.text}>Existe una actualización pendiente. Ingrese al Google Play Store para continuar</Text>
      <Button
        containerStyle={styles.buttonContainer}
        style={styles.button}
        titleUpperCase
        title="Cerrar App"
        onPress={() => props.handleBackPress()}
      />
      <Button
        containerStyle={styles.buttonContainer}
        titleUpperCase
        title="Ingresar a Play Store"
        onPress={() => Linking.openURL('market://details?id=com.jlcvisitortracking')}
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
    fontSize: 20,
    marginBottom: 20
  },
  buttonContainer: {
    padding: 0,
    marginBottom: 20
  }
})

export default OutdatedScreen
