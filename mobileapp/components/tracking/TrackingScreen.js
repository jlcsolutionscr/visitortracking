import React from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import { setScannerActive } from '../../store/ui/actions'

import {
  setBranch,
  validateCodeInfo,
  setNotInList,
  setIdentifier,
  setSelectedCustomer,
  setSelectedEmployee,
  setSelectedService,
  setRating,
  setComment,
  trackVisitorActivity
} from '../../store/session/actions'

import { ScrollView, View, Text } from 'react-native'

import Dropdown from '../custom/Dropdown'
import TextField from '../custom/TextField'
import Button from '../custom/Button'
import RatingBar from '../custom/RatingBar'

import styles from '../styles'

function TrackingScreen(props) {
  const { branch, notInList, identifier, selectedCustomerId, selectedEmployeeId, selectedServiceId, rating, comment, error } = props
  const customer = selectedCustomerId > 0 ? props.customerList.find(item => item.Id === selectedCustomerId) : null
  const customerList = props.customerList.map(item => {
    return { value: item.Id, label: item.Description }
  })
  const employeeList = props.employeeList.map(item => {
    return { value: item.Id, label: item.Description }
  })
  const serviceList = props.serviceList.map(item => {
    return { value: item.Id, label: item.Description }
  })
  const customerName = customer ? customer.Description : ''
  const buttonEnabled = (identifier !== '' || selectedCustomerId > 0) && selectedEmployeeId > 0 && rating > 0
  const handleNotInList = () => {
    props.setNotInList(true)
    props.setSelectedCustomer(0)
  }
  const handleOnPress = () => {
    props.trackVisitorActivity(selectedEmployeeId, selectedServiceId, rating, comment, selectedCustomerId, identifier)
  }
  return (
    <View key="1" style={styles.subContainer}>
      <View style={[styles.header, { marginTop: 30, marginBottom: 20 }]}>
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
            onChangeText={value => props.setIdentifier(value)}
          />
        )}
        {!notInList && customerList.length > 1 && (
          <Dropdown
            label="Gracias por visitarnos"
            selectedValue={selectedCustomerId}
            items={customerList}
            onValueChange={(itemValue, itemPosition) => props.setSelectedCustomer(itemValue)}
          />
        )}
        {!notInList && customerList.length === 1 && <Text style={styles.specialText}>{'Bienvenido ' + customerName}</Text>}
        {!notInList && <Button title="No aparaces" titleUpperCase containerStyle={{ marginTop: 15 }} onPress={() => handleNotInList()} />}
        <Dropdown
          label="Me atendió"
          selectedValue={selectedEmployeeId}
          items={employeeList}
          onValueChange={(itemValue, itemPosition) => props.setSelectedEmployee(itemValue)}
        />
        <Dropdown
          label="Servicio brindado"
          selectedValue={selectedServiceId}
          items={serviceList}
          onValueChange={(itemValue, itemPosition) => props.setSelectedService(itemValue)}
        />
        <RatingBar label="Califiquenos" maxRating={5} rating={rating} onPress={value => props.setRating(value)} />
        <TextField label="Ingrese su comentario" value={comment} onChangeText={value => props.setComment(value)} />
        <Text style={styles.contentText}>Presione ENVIAR para registrar su visita</Text>
        <Button
          title="Enviar"
          titleUpperCase
          disabled={!buttonEnabled}
          containerStyle={{ marginTop: 15 }}
          onPress={() => handleOnPress()}
        />
        <Button title="Regresar" titleUpperCase onPress={() => props.setBranch(null)} />
        {error !== '' && <Text style={styles.errorText}>{error}</Text>}
      </ScrollView>
    </View>
  )
}

const mapStateToProps = state => {
  return {
    branch: state.session.branch,
    notInList: state.session.notInList,
    employeeList: state.session.employeeList,
    customerList: state.session.customerList,
    serviceList: state.session.serviceList,
    identifier: state.session.identifier,
    selectedCustomerId: state.session.selectedCustomerId,
    selectedEmployeeId: state.session.selectedEmployeeId,
    selectedServiceId: state.session.selectedServiceId,
    rating: state.session.rating,
    comment: state.session.comment,
    error: state.session.trackingError
  }
}

const mapDispatchToProps = dispatch => {
  return bindActionCreators(
    {
      setBranch,
      setScannerActive,
      validateCodeInfo,
      setNotInList,
      setIdentifier,
      setSelectedCustomer,
      setSelectedEmployee,
      setSelectedService,
      setRating,
      setComment,
      trackVisitorActivity
    },
    dispatch
  )
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TrackingScreen)
