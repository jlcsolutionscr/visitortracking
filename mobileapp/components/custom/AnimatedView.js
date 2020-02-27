import React, { Component } from 'react'
import { StyleSheet, Animated } from 'react-native'

export default class AnimatedView extends Component {
  constructor(props) {
    super(props)
    this._opacityValue = new Animated.Value(0)
  }

  async componentDidMount () {
    Animated.timing(this._opacityValue, {
      toValue: 1,
      duration: 500,
      useNativeDriver: true
    }).start()
  }

  render() {
    const opacityValue = this._opacityValue.interpolate({
      inputRange: [0, 0.3, 1],
      outputRange: [0, 0.7, 1],
      extrapolate: 'clamp'
    })
    
    return (
      <Animated.View style={[StyleSheet.absoluteFill, { opacity: opacityValue }]}>
        {this.props.children}
      </Animated.View>
    )
  }
}
