import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'
import { withNavigationFocus } from 'react-navigation'

import { registerCustomer } from '../../../store/session/actions'

import { ScrollView, View, Text } from 'react-native'
import Button from '../../custom/Button'
import TextField from '../../custom/TextField'
import DatePicker from '../../custom/DatePicker'

import styles from '../../styles'

export class SignUpScreen extends Component {
  constructor(props) {
    super(props)
    this.state = {
      identifier: '',
      name: '',
      birthday: '09/02/1981',
      address: '',
      mobile: '',
      email: ''
    }
  }

  componentDidUpdate (nextProps) {
    if (nextProps.isFocused !== this.props.isFocused) {
      this.setState({
        identifier: '',
        name: '',
        birthday: '09/02/1981',
        address: '',
        mobile: '',
        email: ''
      })
    }
  }

  render () {
    const { error } = this.props
    const { identifier, name, birthday, address, mobile, email } = this.state
    let buttonEnabled = identifier !== '' && name !== '' && birthday !== '' && address !== '' && mobile !== '' && email !== ''
    return (<View key='1' style={styles.subContainer}>
      <View style={styles.header}>
        <Text style={styles.title}>Registre su información</Text>
      </View>
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
        <DatePicker
          label='Fecha de nacimiento'
          value={birthday}
          onChange={(birthday) => this.setState({birthday})}
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
    const { identifier, name, birthday, address, mobile, email } = this.state
    const customer = {
      Identifier: identifier,
      Name: name,
      Birthday: birthday,
      Address: address,
      MobileNumber: mobile,
      Email: email
    }
    this.props.registerCustomer(customer)
  }
}

const mapStateToProps = (state) => {
  return {
    error: state.session.error
  }
}

const mapDispatchToProps = (dispatch) => {
  return bindActionCreators({
    registerCustomer
  }, dispatch)
}

export default withNavigationFocus(connect(mapStateToProps, mapDispatchToProps)(SignUpScreen))
