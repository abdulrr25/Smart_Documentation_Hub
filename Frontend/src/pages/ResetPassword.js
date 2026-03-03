import { useState } from "react";
import api from "../services/api";
import { useSearchParams, useNavigate } from "react-router-dom";
import "../styles/ResetPassword.css";

export default function ResetPassword() {
  const [password, setPassword] = useState("");
  const [msg, setMsg] = useState("");
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const token = params.get("token");

  async function handleSubmit(e) {
    e.preventDefault();

    try {
      const res = await api("/api/auth/reset-password", {
        method: "POST",
        body: JSON.stringify({
          token,
          newPassword: password,
        }),
      });

      setMsg(res.message);
      setTimeout(() => navigate("/"), 2000);
    } catch (err) {
      console.error(err);
      setMsg("Invalid or expired reset link");
    }
  }

  return (
    <div className="reset-page">
      <div className="reset-card">
        <h2>Reset Your Password</h2>
        <p className="subtitle">Enter a new secure password</p>

        <form onSubmit={handleSubmit}>
          <input
            type="password"
            placeholder="New Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          <button type="submit">Reset Password</button>
        </form>

        {msg && <p className="reset-message">{msg}</p>}
      </div>
    </div>
  );
}