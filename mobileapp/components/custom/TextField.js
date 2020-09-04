import React, { Component } from 'react'
import { Dimensions, StyleSheet, Text, TextInput, View } from 'react-native'

const { width, height } = Dimensions.get('window')
const rem = width / 411.42857142857144
const remY = height / 683.4285714285714

import { formatCurrency, roundNumber } from '../../utils/formatHelper'

class TextField extends Component {
  constructor(props) {
    super(props)
    this.state = { focus: false }
  }

  render() {
    const containerStyle = { ...styles.container, ...this.props.containerStyle }
    const displayText =
      this.props.value.toString() != ''
        ? this.props.currencyFormat
          ? this.state.focus
            ? this.props.value.toString()
            : formatCurrency(roundNumber(this.props.value, 2), 2)
          : this.props.value.toString()
        : ''
    return (
      <View style={containerStyle}>
        <Text style={styles.label}>{this.props.label}</Text>
        <TextInput
          editable={this.props.editable}
          onFocus={() => this.setState({ focus: true })}
          onBlur={() => this.setState({ focus: false })}
          style={styles.input}
          placeholder={this.props.placeholder}
          maxLength={this.props.maxLength}
          onChangeText={this.props.onChangeText}
          onEndEditing={this.props.onEndEditing}
          value={displayText}
          secureTextEntry={this.props.secureTextEntry ? true : false}
          selectTextOnFocus
          autoCapitalize={this.props.autoCapitalize ? this.props.autoCapitalize : 'none'}
          spellCheck={this.props.spellCheck}
          keyboardType={this.props.keyboardType ? this.props.keyboardType : 'default'}
        />
      </View>
    )
  }
}

const styles = StyleSheet.create({
  container: {
    padding: 10,
    paddingBottom: 5
  },
  label: {
    fontFamily: 'TitilliumWeb-Light',
    fontSize: 18 * rem
  },
  input: {
    fontFamily: 'TitilliumWeb-Light',
    fontSize: 18 * rem,
    borderColor: '#5DBCD2',
    borderBottomWidth: 1,
    padding: 10,
    height: 45 * remY
  }
})

export default TextField
