import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import { setScannerActive } from '../../store/ui/actions'

import { setBranch, validateCodeInfo, trackVisitorActivity } from '../../store/session/actions'

import { ScrollView, View, Text } from 'react-native'

import Dropdown from '../custom/Dropdown'
import TextField from '../custom/TextField'
import Button from '../custom/Button'
import RatingBar from '../custom/RatingBar'

import styles from '../styles'

class TrackingScreen extends Component {
  constructor(props) {
    super(props)
    this.state = {
      notInList: this.props.customerList.length === 0,
      identifier: '',
      selectedCustomerId: this.props.customerList.length > 0 ? this.props.customerList[0].Id : 0,
      selectedEmployeeId: this.props.employeeList.length > 0 ? this.props.employeeList[0].Id : 0,
      selectedServiceId: this.props.serviceList.length > 0 ? this.props.serviceList[0].Id : 0,
      rating: 0,
      comment: ''
    }
  }

  render() {
    const { branch, error } = this.props
    const {
      notInList,
      identifier,
      selectedCustomerId,
      selectedEmployeeId,
      selectedServiceId,
      rating,
      comment
    } = this.state
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
    const buttonEnabled = (identifier !== '' || selectedCustomerId > 0) && selectedEmployeeId > 0 && rating > 0
    return (
      <View key="1" style={styles.subContainer}>
        <View style={[styles.header, , { marginTop: 30, marginBottom: 20 }]}>
          <Text style={styles.title}>{branch.Description}</Text>
        </View>
        <ScrollView keyboardShouldPersistTaps="handled" keyboardDismissMode="on-drag">
          {notInList && (
            <TextField
              label="Identificación"
              placeholder="999999999999"
              maxLength={12}
              keyboardType="numeric"
              value={identifier}
              onChangeText={value => this.setState({ identifier: value })}
            />
          )}
          {!notInList && customerList.length > 1 && (
            <Dropdown
              label="Gracias por visitarnos"
              selectedValue={selectedCustomerId}
              items={customerList}
              onValueChange={(itemValue, itemPosition) => this.setState({ selectedCustomerId: itemValue })}
            />
          )}
          {!notInList && customerList.length === 1 && (
            <Text style={styles.specialText}>{'Bienvenido ' + customerName}</Text>
          )}
          {!notInList && (
            <Button
              title="No aparaces"
              titleUpperCase
              containerStyle={{ marginTop: 15 }}
              onPress={() => this.setState({ notInList: true, selectedCustomerId: 0 })}
            />
          )}
          <Dropdown
            label="Me atendió"
            selectedValue={selectedEmployeeId}
            items={employeeList}
            onValueChange={(itemValue, itemPosition) => this.setState({ selectedEmployeeId: itemValue })}
          />
          <Dropdown
            label="Servicio brindado"
            selectedValue={selectedServiceId}
            items={serviceList}
            onValueChange={(itemValue, itemPosition) => this.setState({ selectedServiceId: itemValue })}
          />
          <RatingBar
            label="Califiquenos"
            maxRating={5}
            rating={rating}
            onPress={value => this.setState({ rating: value })}
          />
          <TextField
            label="Ingrese su comentario"
            value={comment}
            onChangeText={value => this.setState({ comment: value })}
          />
          <Text style={styles.contentText}>Presione ENVIAR para registrar su visita</Text>
          <Button
            title="Enviar"
            titleUpperCase
            disabled={!buttonEnabled}
            containerStyle={{ marginTop: 15 }}
            onPress={() => this.handleOnPress()}
          />
          <Button title="Regresar" titleUpperCase onPress={() => this.props.setBranch(null)} />
          {error !== '' && <Text style={styles.errorText}>{error}</Text>}
        </ScrollView>
      </View>
    )
  }

  handleOnPress() {
    const { identifier, selectedEmployeeId, selectedServiceId, rating, comment, selectedCustomerId } = this.state
    this.props.trackVisitorActivity(
      selectedEmployeeId,
      selectedServiceId,
      rating,
      comment,
      selectedCustomerId,
      identifier
    )
  }
}

const mapStateToProps = state => {
  return {
    branch: state.session.branch,
    employeeList: state.session.employeeList,
    customerList: state.session.customerList,
    serviceList: state.session.serviceList,
    error: state.session.trackingError
  }
}

const mapDispatchToProps = dispatch => {
  return bindActionCreators(
    {
      setBranch,
      setScannerActive,
      validateCodeInfo,
      trackVisitorActivity
    },
    dispatch
  )
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TrackingScreen)
