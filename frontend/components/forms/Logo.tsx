import React, { memo } from 'react';
import { Image, StyleSheet } from 'react-native';

const Logo = () => (
  <Image source={require('../../assets/logo.png')} style={styles.image} />
);

const styles = StyleSheet.create({
  image: {
    width: 96,
    height: 96,
    marginBottom: 12,
  },
});

export default memo(Logo);