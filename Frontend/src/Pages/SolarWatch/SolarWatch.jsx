import "./SolarWatch.css";
import { useAuth } from "../Layout/Layout";
import SolarWatchMiddle from "../../Components/SolarWatchMiddle/SolarWatchMiddle";
import { useState, useEffect } from "react";

export default function Home() {
    const [cityName, setCityName] = useState("");
    const [cityData, setCityData] = useState(null);
    const [noCityReq, setNoCityReq] = useState(false);

    const { logout, user } = useAuth();

    const handleClick = (e) => {
        e.preventDefault();
        logout();
    }

    async function fetchCity() {
        console.log(cityName); 
        try {
            const res = await fetch(`api/geocode/${cityName.toLowerCase()}`, {
                headers: {
                    Authorization: `Bearer ${user.token}`
                }
            });
            console.log(res)
            if (!res.ok) {
                throw new Error('Failed to fetch city data');
            }
            const data = await res.json(); 
            setCityData(data);
            setNoCityReq(false);
            console.log(cityData); 
        } catch (err) {
            console.error(err);
        }
    }
    
    useEffect(() => {
        console.log("City data updated:", cityData);
    }, [cityData]);

    async function handleSubmit(e) {
        e.preventDefault();
        await fetchCity();
    }

    return (
        <div className="wrapper">
            <header className="header">
                <h1 className="headertext">SolarWatch</h1>
            </header>
            <div className="navbar">
                <form onSubmit={handleSubmit}>
                    <input className="city-input" required placeholder="City" value={cityName} onChange={(e) => setCityName(e.target.value)}></input>
                    <button className="city-button" type="submit">Get information</button>
                </form>
                <button className="logout-button" onClick={handleClick}>Logout</button>
            </div>
            <SolarWatchMiddle cityName={cityName} cityData={cityData} user={user} noCityReq={noCityReq} setNoCityReq={setNoCityReq}></SolarWatchMiddle>
        </div>
    )
}
