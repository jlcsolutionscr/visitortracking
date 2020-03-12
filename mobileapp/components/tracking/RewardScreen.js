import React, { Component } from 'react'
import { StyleSheet, Animated, View, Text, Easing } from 'react-native'
import Button from '../custom/Button'

import styles from '../styles'

export default class RewardScreen extends Component {
  constructor(props) {
    super(props)
    this.animationValue = new Animated.Value(0.1)
    this.spinValue = new Animated.Value(0.1)
  }

  async componentDidMount () {
    Animated.spring(this.animationValue,
    {
      toValue: 1,
      friction: 3,
      tension: 20
    }).start()
    Animated.timing(
      this.spinValue,
      {
        delay: 150,
        toValue: 1,
        duration: 2500,
        easing: Easing.bounce
      }
    ).start()
  }

  render() {
    const rotateY = this.spinValue.interpolate({
      inputRange: [0.1, 0.25, 0.5, 0.75, 1],
      outputRange: ['0deg', '180deg', '0deg', '180deg', '0deg']
    })
    return (
      <Animated.View style={[StyleSheet.absoluteFill, {transform: [{scale: this.animationValue}]}]}>
        <View style={[styles.header, {marginTop: 30}]}>
          <Text style={styles.title}>{this.props.branch.Description}</Text>
        </View>
        <View style={{margin: 30}}>
          <Text style={styles.contentText}>{this.props.rewardMessage}</Text>
        </View>
        <View style={{alignItems: 'center'}}>
          <Animated.Image
            style={{
              width: 150,
              height: 150,
              transform: [{rotateY}, {scale: this.spinValue}]}}
              source={require('../../assets/prize.png')}
          />
        </View>
        <Button
          title="Cerrar"
          titleUpperCase
          containerStyle={{marginTop: 30}}
          onPress={() => this.props.handleClosePress()}
        />
      </Animated.View>
    )
  }
}
