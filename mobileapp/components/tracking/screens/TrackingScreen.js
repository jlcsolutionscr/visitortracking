import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import { setScannerActive } from '../../../store/ui/actions'

import {
  setBranch,
  validateCodeInfo,
  customerRegistration,
  trackVisitorActivity
} from '../../../store/session/actions'

import { View, Text } from 'react-native'

import Dropdown from '../../custom/Dropdown'
import Button from '../../custom/Button'
import RatingBar from '../../custom/RatingBar'

import styles from '../../styles'

class TrackingScreen extends Component {
  constructor (props) {
    super(props)
    this.state = {
      selectedCustomerId: this.props.customerList.length > 0 ? this.props.customerList[0].Id : 0,
      selectedEmployeeId: this.props.employeeList.length > 0 ? this.props.employeeList[0].Id : 0,
      selectedServiceId: this.props.serviceList.length > 0 ? this.props.serviceList[0].Id : 0,
      rating: 0
    }
  }

  render () {
    const { branch, error } = this.props
    const { selectedCustomerId, selectedEmployeeId, selectedServiceId, rating } = this.state
    const customer = this.props.customerList.find(item => item.Id === selectedCustomerId)
    const customerList = this.props.customerList.map(item => {
      return { value: item.Id, label: item.Description }
    })
    const employeeList = this.props.employeeList.map(item => {
      return { value: item.Id, label: item.Description }
    })
    const serviceList = this.props.serviceList.map(item => {
      return { value: item.Id, label: item.Description }
    })
    const customerName = customer ? customer.Description : ''
    const buttonEnabled = selectedCustomerId > 0 && selectedEmployeeId > 0
    return <View key='1' style={styles.subContainer}>
      <View style={styles.header}>
        <Text style={styles.title}>{branch.Description}</Text>
      </View>
      {customerList.length > 1 &&<Dropdown
        label='Gracias por visitarnos'
        selectedValue={selectedCustomerId}
        items={customerList}
        onValueChange={(itemValue, itemPosition) => this.setState({selectedCustomerId: itemValue})}
      />}
      {customerList.length === 1 && <Text style={styles.specialText}>{'Bienvenido ' + customerName}</Text>}
      <Dropdown
        label='Me atendió'
        selectedValue={selectedEmployeeId}
        items={employeeList}
        onValueChange={(itemValue, itemPosition) => this.setState({selectedEmployeeId: itemValue})}
      />
      <Dropdown
        label='Servicio brindado'
        selectedValue={selectedServiceId}
        items={serviceList}
        onValueChange={(itemValue, itemPosition) => this.setState({selectedServiceId: itemValue})}
      />
      <RatingBar
        label='Califiquenos'
        maxRating={5}
        rating={rating}
        onPress={(value) => this.setState({rating: value})}
      />
      <Text style={styles.contentText}>Presione ENVIAR para registrar su visita</Text>
      <Button
        title="Enviar"
        titleUpperCase
        disabled={!buttonEnabled}
        containerStyle={{marginTop: 15}} 
        onPress={() => this.handleOnPress()}
      />
      <Button
        title="Regresar"
        titleUpperCase
        disabled={!buttonEnabled}
        onPress={() => this.props.setBranch(null)}
      />
      {error !== '' && <Text style={styles.errorText}>{error}</Text>}
    </View>
  }

  handleOnPress() {
    const { selectedEmployeeId, selectedServiceId, rating, selectedCustomerId } = this.state
    this.props.trackVisitorActivity(selectedEmployeeId, selectedServiceId, rating, selectedCustomerId)
  }
}

const mapStateToProps = (state) => {
  return {
    branch: state.session.branch,
    employeeList: state.session.employeeList,
    customerList: state.session.customerList,
    serviceList: state.session.serviceList,
    error: state.session.error
  }
}

const mapDispatchToProps = (dispatch) => {
  return bindActionCreators({
    setBranch,
    setScannerActive,
    validateCodeInfo,
    customerRegistration,
    trackVisitorActivity
  }, dispatch)
}

export default connect(mapStateToProps, mapDispatchToProps)(TrackingScreen)
