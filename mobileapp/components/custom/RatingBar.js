import React from 'react'

import { StyleSheet, Dimensions, View, Text, Image, TouchableOpacity } from 'react-native'
const { width } = Dimensions.get('window')
const rem = width / 411.42857142857144

const RatingBar = (props) => {
  let React_Native_Rating_Bar = [];
  for (var i = 1; i <= props.maxRating; i++) {
    React_Native_Rating_Bar.push(
      <TouchableOpacity
        activeOpacity={0.7}
        key={i}
        onPress={props.onPress.bind(this, i)}>
        <Image
          style={styles.StarImage}
          source={
            i <= props.rating
              ? require('../../assets/star-filled.png')
              : require('../../assets/star-unfilled.png')
          }
        />
      </TouchableOpacity>
    );
  }
  return (
    <View style={styles.container}>
      <Text style={styles.labelStyle}>{props.label}</Text>
      <View style={styles.childView}>{React_Native_Rating_Bar}</View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    padding: 10,
    paddingBottom: 5
  },
  childView: {
    justifyContent: 'center',
    flexDirection: 'row',
    marginTop: 5
  },
  StarImage: {
    width: 40,
    height: 40,
    margin: 5,
    resizeMode: 'cover'
  },
  labelStyle: {
    fontFamily: 'TitilliumWeb-Regular',
    fontSize: (20 * rem),
    textAlign: 'center'
  }
})

export default RatingBar
