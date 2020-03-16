import React from 'react'
import { Dimensions, StyleSheet, View, TouchableOpacity, Text } from 'react-native'

const { width, height } = Dimensions.get('window')
const rem = width / 411.42857142857144
const remY = height / 683.4285714285714

const Button = (props) => {
  const containerStyles = {
    ...styles.container,
    ...props.containerStyle
  }
  const elementStyles = {
    ...styles.button,
    borderColor: props.disabled ? '#DCDCDC' : '#5DBCD2',
    backgroundColor: props.disabled ? '#DCDCDC' : '#5DBCD2'
  }
  const textStyles = {
    ...styles.text,
    color: props.disabled ? '#909596' : 'white'
  }
  const label = props.titleUpperCase ? props.title.toUpperCase() : props.title
  return (
    <View style={containerStyles}>
      <TouchableOpacity
        style={elementStyles}
        disabled={props.disabled ? props.disabled : false}
        activeOpacity={0.8}
        onPress={props.onPress}>
        <Text style={textStyles}>{label}</Text>
      </TouchableOpacity>
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    justifyContent: 'center',
    padding: 10,
    marginLeft: 25,
    marginRight: 25
  },
  button: {
    justifyContent: 'center',
    alignItems: 'center',
    paddingLeft: 10,
    paddingRight: 10,
    borderWidth: 1,
    borderRadius: 20,
    height: (50 * remY)
  },
  text: {
    fontFamily: 'TitilliumWeb-SemiBold',
    fontSize: (18 * rem)
  }
})

export default Button
