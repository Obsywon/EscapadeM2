import axios from "axios";
import env from "../env";

const BASE_URL = env.GOOGLE_BASE_URL;
const API_KEY = env.GOOGLE_API_KEY;

export default async function nearByPlace(
  lat: number,
  lng: number,
  type: string
) {
  // axios.get(`${BASE_URL}nearbysearch/json?location=-33.8670522%2C151.1957362&radius=1500&type=restaurant&key=${API_KEY}`);
  return await axios.get(
    `${BASE_URL}nearbysearch/json?location=${lat},${lng}&radius=1500&type=${type}&key=${API_KEY}`
  );
}
