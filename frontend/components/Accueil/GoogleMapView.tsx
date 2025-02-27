import React, { useState, useContext, useEffect } from 'react';
import { View, Dimensions, StyleSheet, Linking, TouchableOpacity } from 'react-native';
import MapView, { Marker, PROVIDER_GOOGLE, Region } from 'react-native-maps';
import MapViewDirections from 'react-native-maps-directions';
import PlaceMarker from './PlaceMarker';
import { API_KEY } from '../../services/GlobaleApi';
import { UserLocationContextType, UserLocationContext } from '../../contexts/UserLocationContext';
import CustomMarkerImage from '../../assets/logo.png';
import { TransportationMode } from '../../models/TransportationMode';
import { Surface, Text } from 'react-native-paper';
import { CustomColors } from '../../themes/CustomColors';

interface GoogleMapViewProps {
  placeList?: any[];
  userLocationContext?: UserLocationContextType;
  transportMode?: TransportationMode;
  onSecondMapMarkerChange?: (marker: { latitude: number; longitude: number } | undefined) => void;
}

export default function GoogleMapView({ placeList, transportMode, onSecondMapMarkerChange }: Readonly<GoogleMapViewProps>) {
  const [mapRegion, setMapRegion] = useState<Region>({
    latitude: 49.1193,
    longitude: 6.1727,
    latitudeDelta: 0.0522,
    longitudeDelta: 0.0421,
  });

  const userLocationContext = useContext<UserLocationContextType>(UserLocationContext);

  const [startPoint, setStartPoint] = useState<{ latitude: number; longitude: number } | undefined>(undefined);
  const [secondMapMarker, setSecondMapMarker] = useState<{ latitude: number; longitude: number } | undefined>(undefined);

  useEffect(() => {
    if (userLocationContext?.location) {
      setMapRegion({
        latitude: userLocationContext.location.coords.latitude,
        longitude: userLocationContext.location.coords.longitude,
        latitudeDelta: 0.0922,
        longitudeDelta: 0.0421,
      });

      setStartPoint({
        latitude: userLocationContext.location.coords.latitude,
        longitude: userLocationContext.location.coords.longitude,
      });
    }
  }, [userLocationContext]);

  const extractCoordinates = (place: { geometry: { location: { lat: any; lng: any; }; }; }) => ({
    latitude: place?.geometry?.location?.lat || 0,
    longitude: place?.geometry?.location?.lng || 0,
  });

  if(placeList && transportMode){
    const waypoints = placeList.slice(0, 10).map(extractCoordinates);
 
    const handleNavigatePress = () => {
      const waypointCoords = waypoints.map((waypoint) => `${waypoint.latitude},${waypoint.longitude}`).join('|');
    
      Linking.openURL(`https://www.google.com/maps/dir/?api=1&origin=${startPoint?.latitude},${startPoint?.longitude}&destination=${startPoint?.latitude},${startPoint?.longitude}&waypoints=${waypointCoords}&travelmode=${transportMode.toLowerCase()}`);
    };
  
    return (
      <View style={styles.container}>
        
        <Surface style={styles.mapContainer} mode='elevated' elevation={2}>
        <Text style={styles.texteTitre}>Meilleurs endroits à proximité</Text>
          <MapView style={styles.map} provider={PROVIDER_GOOGLE} showsUserLocation={true} region={mapRegion}>
            {startPoint && <Marker coordinate={startPoint} image={CustomMarkerImage} />}
            {placeList.map((item, index) => index < 10 && <PlaceMarker key={index} item={item} />)}
            <MapViewDirections
              origin={startPoint}
              waypoints={waypoints}
              destination={startPoint}
              apikey={API_KEY}
              strokeWidth={3}
              strokeColor="hotpink"
              mode={transportMode}
            />
          </MapView>
        </Surface>
        <TouchableOpacity style={styles.navigateButton} onPress={handleNavigatePress}>
          <Text style={styles.navigateButtonText}>Naviguer</Text>
        </TouchableOpacity>
      </View>
    );
  } else {

    const handleSecondMapPress = (event: any) => {
      const { coordinate } = event.nativeEvent;
      const newMarker = {
        latitude: coordinate.latitude,
        longitude: coordinate.longitude,
      };
      setSecondMapMarker(newMarker);
      onSecondMapMarkerChange && onSecondMapMarkerChange(newMarker);
    };
    
    return (
      <View style={styles.container}>
        <Surface style={styles.mapContainer} mode='elevated' elevation={2}>
        <MapView
          style={styles.secondMap}
          provider={PROVIDER_GOOGLE}
          showsUserLocation={true}
          region={mapRegion}
          onPress={handleSecondMapPress} 
        >
          {startPoint && <Marker coordinate={startPoint} image={CustomMarkerImage} />}
          {secondMapMarker && (
            <Marker
              coordinate={secondMapMarker}
            />
          )}
        </MapView>
        </Surface>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: 8,
    borderRadius: 16,
    
  },
  texteTitre: {
    fontSize: 16,
    marginBottom: 10,
  },
  mapContainer: {
    padding: 8,
    borderRadius: 16,
    backgroundColor: 'white'
  },
  map: {
    width: Dimensions.get('screen').width * 0.89,
    height: Dimensions.get('screen').height * 0.23,
  },
  secondMap: {
    width: Dimensions.get('screen').width * 1,
    height: Dimensions.get('screen').height * 1,
  },
  navigateButton: {
    backgroundColor: CustomColors.inputOutline,
    padding: 8,
    borderRadius: 8,
    marginTop: 8,
    alignItems: 'center',
  },
  navigateButtonText: {
    color: 'white',
    fontSize: 16,
  },
});
