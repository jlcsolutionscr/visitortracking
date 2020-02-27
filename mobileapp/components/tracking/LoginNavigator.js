import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import { Image } from 'react-native'
import { createAppContainer } from 'react-navigation'
import { createBottomTabNavigator } from 'react-navigation-tabs'
import AnimatedView from '../custom/AnimatedView'

import SignUpScreen from './screens/SignUpScreen'
import TrackingScreen from './screens/TrackingScreen'

import { setBranch } from '../../store/session/actions'
import styles from '../styles'

class LoginNavigator extends Component {
  render () {
    let tabs = {
      SignUp: SignUpScreen
    }
    if (this.props.customerList.length > 0) {
      tabs = {
        Track: TrackingScreen,
        SignUp: SignUpScreen
      }
    }
    const TabNavigator = createBottomTabNavigator(
    tabs,
    {
      defaultNavigationOptions: ({ navigation }) => ({
        tabBarIcon: ({ focused, horizontal, tintColor }) => {
          const { routeName } = navigation.state;
          if (routeName === 'Track') {
            return (
              <Image
                source={require('../../assets/user-white.png')}
                style={styles.navIcon} />
            );
          } else {
            return (
              <Image
                source={require('../../assets/settings-white.png')}
                style={styles.navIcon} />
            );
          }
        },
      }),
      tabBarOptions: {
        style: {
          backgroundColor: 'black',
        },
        activeTintColor: 'white',
        inactiveTintColor: 'gray',
      },
    });
    const AppContainer = createAppContainer(TabNavigator)
    return (
      <AnimatedView>
        <AppContainer />
      </AnimatedView>
    )
  }
}

const mapStateToProps = (state) => {
  return {
    branch: state.session.branch,
    customerList: state.session.customerList
  }
}

const mapDispatchToProps = (dispatch) => {
  return bindActionCreators({
    setBranch
  }, dispatch)
}

export default connect(mapStateToProps, mapDispatchToProps)(LoginNavigator)
