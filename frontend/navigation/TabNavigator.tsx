import React from "react";
import { createMaterialBottomTabNavigator } from "react-native-paper/react-navigation";

import { Ionicons, FontAwesome } from "@expo/vector-icons";
import DashboardScreen from "../pages/DashboardScreen";
import RechercheScreen from "../pages/RechercheScreen";
import FavoriScreen from "../pages/FavoriScreen";
import PhotosScreen from "../pages/PhotosScreen";
import ProfileScreen from "../pages/ProfileScreen";
import { CustomColors } from "../themes/CustomColors";

const iconSize: number = 26;

export type BottomTabParamList = {
  Accueil: undefined;
  Recherche: undefined;
  Favori: undefined;
  Photos: undefined;
  Profil: undefined;
};

type IconProps = {
  color: string;
  size?: number;
};

type TabElement = {
  label: keyof BottomTabParamList;
  component: (T: any) => JSX.Element;
  iconComponent?: ({ color, size }: IconProps) => JSX.Element;
};

const tabs: TabElement[] = [
  {
    label: "Accueil",
    component: DashboardScreen,
    iconComponent: ({ color }: IconProps) => (
      <Ionicons name="home" color={color} size={iconSize} />
    ),
  },
  {
    label: "Recherche",
    component: RechercheScreen,
    iconComponent: ({ color }: IconProps) => (
      <Ionicons name="search" color={color} size={iconSize} />
    ),
  },
  {
    label: "Favori",
    component: FavoriScreen,
    iconComponent: ({ color }: IconProps) => (
      <Ionicons name="ios-heart" color={color} size={iconSize} />
    ),
  },
  {
    label: "Photos",
    component: PhotosScreen,
    iconComponent: ({ color }: IconProps) => (
      <FontAwesome name="photo" color={color} size={iconSize} />
    ),
  },
  {
    label: "Profil",
    component: ProfileScreen,
    iconComponent: ({ color }: IconProps) => (
      <FontAwesome name="user-circle-o" color={color} size={iconSize} />
    ),
  },
];

export default function TabNavigator(): JSX.Element {
  const Tab = createMaterialBottomTabNavigator<BottomTabParamList>();

  return (
    <Tab.Navigator 
    shifting={true}
    compact={true}
    activeColor={CustomColors.secondary}
    inactiveColor={CustomColors.AppTitleColor}
    initialRouteName="Dashboard"
    >
      {tabs.map((tab) => (
        <Tab.Screen
          key={tab.label}
          name={tab.label}
          component={tab.component}
          options={{
            tabBarLabel: tab.label,
            tabBarIcon: tab.iconComponent,
          }}
        />
      ))}
    </Tab.Navigator>
  );
}
