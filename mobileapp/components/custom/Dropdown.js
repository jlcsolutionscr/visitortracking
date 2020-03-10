import React from 'react'
import { Dimensions, StyleSheet, View, Text } from 'react-native'
import { Picker } from '@react-native-community/picker'

const { width, height } = Dimensions.get('window')
const rem = width / 411.42857142857144
const remY = height / 683.4285714285714

const Dropdown = (props) => {
  const pickers = props.items.map((item, index) => {
    return <Picker.Item key={index} value={item.value} label={item.label} />
  })
  return (
    <View style={styles.container}>
      {props.label && <Text style={styles.label}>
        {props.label}
      </Text>}
      <Picker
        enabled={props.disabled ? !props.disabled : true}
        selectedValue={props.selectedValue}
        onValueChange={props.onValueChange}
        prompt='Seleccione una opción'
      >
        {pickers}
      </Picker>
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    padding: 10,
    paddingBottom: 1
  },
  label: {
    fontFamily: 'TitilliumWeb-Regular',
    fontSize: (20 * rem),
    textAlign: 'center'
  },
  item: {
    color: 'red',
    fontFamily: 'TitilliumWeb-Light',
    fontSize: (10 * rem),
    height: (50 * remY),
    padding: 10
  }
})

export default Dropdown
