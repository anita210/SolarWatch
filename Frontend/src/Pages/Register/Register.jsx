import { useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import "../Login/Login.css";
import { AuthContext } from "../Layout/Layout.jsx";
import RegisterInput from "../../Components/RegisterInput/RegisterInput.jsx";

export default function Register() {

    const { user } = useContext(AuthContext);
    const [formData, setFormData] = useState({
        userName: "",
        email: "",
        password: "",
    })
    const navigate = useNavigate();

    async function registerUser() {
        try {
            const res = await fetch(`api/auth/register`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(formData)
            })
            if (!res.ok) {
                throw new Error("Cant register user")
            }
            navigate("/login");
        }
        catch(e){
            console.error(e);
        }
    }



    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!user){
            console.log(`username: ${formData.userName} email: ${formData.email}  password: ${formData.password}"`);
            await registerUser();
        }
        else{
            navigate("/login");
        }
    };

    return (
        <div className="loginbody">
            <div className="wrapper">
                <header className="header">
                    <h1 className="headertext">SolarWatch</h1>
                </header>
                <div className="navbar" id="login-navbar">
                    <div className="login-container">
                        <RegisterInput handleSubmit={handleSubmit} formData={formData} setFormData={setFormData}></RegisterInput>
                    </div>
                </div>

            </div>

        </div>
    )
}