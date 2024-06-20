import { useNavigate } from "react-router-dom";
import "../Login/Login.css";
export default function Home() {
    const navigate = useNavigate();
    return (
        <div className="loginbody">
            <div className="wrapper">
                <header className="header">
                    <h1 className="headertext">SolarWatch</h1>
                </header>
                <div className="navbar" id="login-navbar">
                    <div className="login-container">
                        <h3>Welcome!</h3>
                        <h6>Login or register to continue</h6>
                        <button className="welcomebutton" onClick={() => navigate("/login")}>Login</button>
                        <button className="welcomebutton" onClick={() => navigate("/register")}>Register</button>
                    </div>
                </div>
            </div>
        </div>
    )
}