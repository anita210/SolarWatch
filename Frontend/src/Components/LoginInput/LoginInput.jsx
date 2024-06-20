export default function LoginInput({handleSubmit, formData, setFormData}){
    return(
        <div>

            <form onSubmit={(e) =>handleSubmit(e)}>
                <div>
                    <label htmlFor="email">Email:</label>
                    <input
                    className="login-input"
                    type="email"
                    id="email"
                    value={formData.email}
                    onChange={(e) => setFormData({...formData, email: e.target.value})}
                    required
                    />
                </div>
                <div>
                    <label htmlFor="password" >Password:</label>
                    <input
                    className="login-input"
                    type="password"
                    id="password"
                    value={formData.password}
                    onChange={(e)=> setFormData({...formData, password: e.target.value})}
                    required
                    />
                </div>
                <button type="submit" className="loginbutton">Login</button>
            </form>
            <a href="/register" >Create an account</a>
        </div>
    )
}