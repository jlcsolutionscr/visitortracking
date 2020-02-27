import React, { Component } from 'react'
import { StyleSheet, Animated, Image, Dimensions } from 'react-native'
import LinearGradient from 'react-native-linear-gradient'


const { width, height } = Dimensions.get('window')
const rem = width / 411.42857142857144
const remY = height / 683.4285714285714


export default class SplashScreen extends Component {
  constructor(props) {
    super(props)
    this._hideValue = new Animated.Value(0)
  }

  async componentDidMount () {
    Animated.timing(this._hideValue, {
      toValue: 1,
      delay: 500,
      duration: 500,
      useNativeDriver: true
    }).start(() => {
      this.props.onCompleted()
    })
  }

  render() {
    const opacityVisibleToClear = {
      opacity: this._hideValue.interpolate({
        inputRange: [0, 0.5, 1],
        outputRange: [1, 0.7, 0],
        extrapolate: 'clamp'
      })
    }
    
    return (
      <Animated.View style={[StyleSheet.absoluteFill, opacityVisibleToClear]}>
        <LinearGradient colors={['#0792C4', '#0D496E']} style={[styles.container, StyleSheet.absoluteFill]}>
          <Image
            source={require("../../assets/inicio.png")}
            style={styles.image}
          />
          <Image
            source={require("../../assets/android-logo.png")}
            style={styles.logo}
          />
        </LinearGradient>
        
      </Animated.View>
    )
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center'
  },
  logo: {
    height: (50 * remY),
    width: (50 * rem),
    position: 'absolute',
    bottom: height * 0.10
  },
  image: {
    width: 250,
    height: 166.66
  }
})
