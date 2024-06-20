import { useState } from "react"

export default function SolarWatchMiddle({ cityName, cityData, user, noCityReq, setNoCityReq }) {
    const [sunriseDate, setSunriseDate] = useState("2023-04-23");
    const [sunsetDate, setSunsetDate] = useState("2023-04-23");
    const [dataSunrise, setDataSunrise] = useState(null);
    const [dataSunset, setDataSunset] = useState(null);
  

    const fetchSolar = async (e, cityName, solar, sunDate, setData) => {
        try {
            const res = await fetch(`api/solarwatch/${solar}/${cityName}/${sunDate}`, {
                headers: {
                    Authorization: `Bearer ${user.token}`
                }
            });
            if (!res.ok) {
                throw new Error('Failed to fetch sunrise data');
            }
            const data = await res.json();
            console.log("data:" + { data });
            setData(data);
        } catch (error) {
            console.error('Error fetching sunrise data:', error);
            return null;
        }
    };


    const handleClick = async (e, cityName, solar, sunDate, setData) => {
        e.preventDefault();
        if (cityName) {
            await fetchSolar(e, cityName, solar, sunDate, setData);
            setNoCityReq(false);
        }
        else {
            setNoCityReq(true);
        }

    }


    return (
        <div className="middlepage">
            <div className="sunrise">
                <form onSubmit={(e) => handleClick(e, cityName, "sunrise", sunriseDate, setDataSunrise)}>
                    <button className="sunrise-button" type="submit" >Sunrise</button>
                    <input className="sunrise-input" type="date" required value={sunriseDate} onChange={(e) => setSunriseDate(e.target.value)}></input>
                </form>
                <div className="sunrisedata-container">
                    {noCityReq ?
                        <div>
                            <h4>No city specified</h4>
                        </div> :
                        <div>
                            <h4>{dataSunrise && dataSunrise.sunriseTime.slice(-8)}</h4>
                        </div>}
                </div>
            </div>
            <div className="city">
                <div className="citydata-container">
                    {cityData &&
                        <div>
                            <h2>{cityData.name}</h2>
                            <h4>State</h4>
                            <h3>{cityData.state ? cityData.state : "No state available"}</h3>
                            <h4>Country</h4>
                            <h3>{cityData.country ? cityData.country : "No country available"}</h3>
                            <h4>Longitude</h4>
                            <h3>{cityData.longitude.toString().slice(0, 5)}</h3>
                            <h4>Latitude</h4>
                            <h3>{cityData.latitude.toString().slice(0, 5)}</h3>
                        </div>}
                </div>
            </div>
            <div className="sunset">
                <form onSubmit={(e) => handleClick(e, cityName, "sunset", sunsetDate, setDataSunset)}>
                    <button className="sunset-button" type="submit">Sunset</button>
                    <input className="sunset-input" type="date" required value={sunsetDate} onChange={(e) => setSunsetDate(e.target.value)}></input>
                </form>
                <div className="sunsetdata-container">
                    {noCityReq ?
                        <div>
                            <h4>No city specified</h4>
                        </div> :
                        <div>
                            <h4>{dataSunset && dataSunset.sunsetTime.slice(-8)}</h4>
                        </div>}
                </div>
            </div>
        </div>
    )
}
