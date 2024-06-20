import { createContext, useContext, useMemo, useEffect, useState} from "react";
import { Outlet, useNavigate } from "react-router-dom";
import {useLocalStorage} from "../../Components/LocalStorage/LocalStorage";


export const AuthContext = createContext(null);

export default function Layout() {
    const [user, setUser] = useLocalStorage("user", null);
    const navigate = useNavigate();

    const login = async (data) => {
        try {
            let response = await fetch("/api/Auth/Login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data)
            });
            if (response.status === 400) {
                console.error("Bad request. Check your input");
                return false;
            }
            let user = await response.json();
            console.log("Logged in user" + user);
            setUser(user);
            navigate("/solar-watch");
        }
        catch (err) {
            console.log(err)
        }
    }

    //replace prevents logged out user to navigate back
    const logout = () => {
        setUser(null);
        navigate("/login", { replace: true });
    }

    const value = useMemo(
        () => ({
            user, login, logout
        }), [user]
    )

    return (
        <AuthContext.Provider value={value}>
            <Outlet>
            </Outlet>
        </AuthContext.Provider>
    )
}

export const useAuth = () => {
    return useContext(AuthContext);
  };

