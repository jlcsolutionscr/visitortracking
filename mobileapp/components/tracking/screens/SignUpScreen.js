import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'
import { withNavigationFocus } from 'react-navigation'

import { registerCustomer } from '../../../store/session/actions'

import { ScrollView, View, Text } from 'react-native'
import Dropdown from '../../custom/Dropdown'
import Button from '../../custom/Button'
import TextField from '../../custom/TextField'

import styles from '../../styles'

export class SignUpScreen extends Component {
  constructor(props) {
    super(props)
    this.state = {
      selectedEmployeeId: this.props.employeeList.length > 0 ? this.props.employeeList[0].Id : 0,
      identifier: '',
      name: '',
      address: '',
      mobile: '',
      email: ''
    }
  }

  componentDidUpdate (nextProps) {
    if (nextProps.isFocused !== this.props.isFocused) {
      this.setState({
        selectedEmployeeId: this.props.employeeList.length > 0 ? this.props.employeeList[0].Id : 0,
        identifier: '',
        name: '',
        address: '',
        mobile: '',
        email: ''
      })
    }
  }

  render () {
    const { error } = this.props
    const { selectedEmployeeId, identifier, name, address, mobile, email } = this.state
    let buttonEnabled = selectedEmployeeId > 0 && identifier !== '' && name !== '' && address !== '' && mobile !== '' && email !== ''
    const employeeList = this.props.employeeList.map(item => {
      return { value: item.Id, label: item.Description }
    })
    return (<View key='1' style={styles.subContainer}>
      <View style={styles.header}>
        <Text style={styles.title}>Registro de clientes</Text>
      </View>
      <Dropdown
        label='Me atendió'
        selectedValue={selectedEmployeeId}
        items={employeeList}
        onValueChange={(itemValue, itemPosition) => this.setState({selectedEmployeeId: itemValue})}
      />
      <ScrollView keyboardShouldPersistTaps='handled'>
        <TextField
          label='Identificación'
          placeholder='999999999999'
          maxLength={12}
          keyboardType='numeric'
          value={identifier}
          onChangeText={(identifier) => this.setState({identifier})}
        />
        <TextField
          label='Nombre'
          placeholder='Nombre del cliente'
          value={name}
          onChangeText={(name) => this.setState({name})}
        />
        <TextField
          label='Dirección'
          placeholder='Dirección'
          value={address}
          onChangeText={(address) => this.setState({address})}
        />
        <TextField
          label='Teléfono'
          placeholder='99999999'
          keyboardType='numeric'
          value={mobile}
          onChangeText={(mobile) => this.setState({mobile})}
        />
        <TextField
          label='Correo'
          placeholder='xxx@yyyy.zzz'
          value={email}
          onChangeText={(email) => this.setState({email})}
        />
        <Button
          title="Enviar"
          titleUpperCase
          disabled={!buttonEnabled}
          containerStyle={{marginTop: 20}} 
          onPress={() => this.handleOnPress()}
        />
        {error !== '' && <Text style={styles.errorText}>{error}</Text>}
      </ScrollView>
    </View>)
  }

  handleOnChange(value) {
    this.props.setSignUpError('')
    this.setState(value)
  }

  async handleOnPress () {
    const { selectedEmployeeId, identifier, name, address, mobile, email } = this.state
    const customer = {
      Identifier: identifier,
      Name: name,
      Address: address,
      MobileNumber: mobile,
      Email: email
    }
    this.props.registerCustomer(selectedEmployeeId, customer)
  }
}

const mapStateToProps = (state) => {
  return {
    employeeList: state.session.employeeList,
    error: state.session.error
  }
}

const mapDispatchToProps = (dispatch) => {
  return bindActionCreators({
    registerCustomer
  }, dispatch)
}

export default withNavigationFocus(connect(mapStateToProps, mapDispatchToProps)(SignUpScreen))
