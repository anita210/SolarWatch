import {useContext, useState} from "react";
import LoginInput from "../../Components/LoginInput/LoginInput.jsx";
import useSignIn from "react-auth-kit/hooks/useSignIn";
import {useNavigate} from "react-router-dom";
import "./Login.css";
import { AuthContext } from "../Layout/Layout.jsx";

export default function Login()
{

    const {login} = useContext(AuthContext);
    const [formData, setFormData] = useState({
        email: "",
        password: "",
    })
    
    const handleSubmit = async (e) => {
        e.preventDefault();
        console.log(`email: ${formData.email}  password: ${formData.password}"`);
        await login(formData);
    };
   
   return(
       <div className="loginbody">
        <div className="wrapper">
            <header className="header">
                <h1 className="headertext">SolarWatch</h1>
            </header>
            <div className="navbar" id="login-navbar">
                <div className="login-container">
            <LoginInput handleSubmit={handleSubmit} formData={formData} setFormData={setFormData}></LoginInput>
            </div>
            </div>           
        </div>
       </div>
   )
}