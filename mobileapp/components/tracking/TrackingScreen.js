import React, { useState } from 'react'
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

function TrackingScreen(props) {
  const { branch, error } = props
  const [notInList, setNotInList] = useState(props.customerList.length === 0)
  const [identifier, setIdentifier] = useState('')
  const [selectedCustomerId, setSelectedCustomerId] = useState(props.customerList.length === 0)
  const [selectedEmployeeId, setSelectedEmployeeId] = useState(props.employeeList.length === 0)
  const [selectedServiceId, setSelectedServiceId] = useState(props.serviceList.length === 0)
  const [rating, setRating] = useState(0)
  const [comment, setComment] = useState('')
  const customer = props.customerList.find(item => item.Id === selectedCustomerId)
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

  const handleNotInListClick = () => {
    setNotInList(true)
    setSelectedCustomerId(0)
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
            onChangeText={value => setIdentifier(value)}
          />
        )}
        {!notInList && customerList.length > 1 && (
          <Dropdown
            label="Gracias por visitarnos"
            selectedValue={selectedCustomerId}
            items={customerList}
            onValueChange={(itemValue, itemPosition) => setSelectedCustomerId(itemValue)}
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
            onPress={() => handleNotInListClick()}
          />
        )}
        <Dropdown
          label="Me atendió"
          selectedValue={selectedEmployeeId}
          items={employeeList}
          onValueChange={(itemValue, itemPosition) => setSelectedEmployeeId(itemValue)}
        />
        <Dropdown
          label="Servicio brindado"
          selectedValue={selectedServiceId}
          items={serviceList}
          onValueChange={(itemValue, itemPosition) => setSelectedServiceId(itemValue)}
        />
        <RatingBar label="Califiquenos" maxRating={5} rating={rating} onPress={value => setRating(value)} />
        <TextField label="Ingrese su comentario" value={comment} onChangeText={value => setComment(value)} />
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
