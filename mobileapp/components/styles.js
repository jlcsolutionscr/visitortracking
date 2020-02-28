import { Dimensions, StyleSheet } from 'react-native'
const { width, height } = Dimensions.get('window')
const rem = width / 411.42857142857144
const remY = height / 683.4285714285714

export default styles = StyleSheet.create({
  container: {
    flex: 1
  },
  subContainer: {
    flex: 1,
    padding: 10
  },
  modal: {
    marginLeft: 10,
    marginRight: 10
  },
  dialogContentView: {
    padding: 20
  },
  modalButtonText: {
    fontFamily: 'TitilliumWeb-Regular',
    fontSize: (16 * rem)
  },
  header: {
    height: 90 * remY,
    backgroundColor: '#FAFAFA',
    justifyContent: 'center',
    alignItems: 'center'
  },
  title: {
    fontFamily: 'TitilliumWeb-Bold',
    fontSize: (22 * rem),
    color: '#004F6C'
  },
  content: {
    backgroundColor: '#FAFAFA',
    padding: 20
  },
  navIcon: {
    width: (20 * rem),
    height: (20 * rem)
  },
  contentText: {
    fontFamily: 'TitilliumWeb-Light',
    fontSize: (18 * rem),
    textAlign: 'center'
  },
  specialText: {
    marginTop: 20,
    marginBottom: 20,
    color: 'blue',
    fontFamily: 'TitilliumWeb-Regular',
    fontSize: (20 * rem),
    textAlign: 'center'
  },
  errorText: {
    fontFamily: 'TitilliumWeb-Light',
    fontSize: (18 * rem),
    color: 'red',
    textAlign: 'center'
  }
})