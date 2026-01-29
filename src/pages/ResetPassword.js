import { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import api from "../api/axios";
import "../styles/Auth.css";

export default function ResetPassword() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const token = searchParams.get("token");

  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!token) {
      setMessage("Invalid or missing reset token");
    }
  }, [token]);

  const handleReset = async () => {
    if (!password) {
      setMessage("Password is required");
      return;
    }

    try {
      setLoading(true);
      setMessage("");

      const res = await api.post("/auth/reset-password", {
        token,
        newPassword: password,
      });

      setMessage(res.data);

      // Redirect to login after success
      setTimeout(() => {
        navigate("/login");
      }, 2000);
    } catch (err) {
      setMessage(
        err.response?.data || "Invalid or expired reset token"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-bg">
      <div className="auth-card">
        <h2>Reset Password</h2>

        {!token ? (
          <p className="auth-error">Invalid reset link</p>
        ) : (
          <>
            <input
              type="password"
              placeholder="Enter new password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            <button onClick={handleReset} disabled={loading}>
              {loading ? "Resetting..." : "Reset Password"}
            </button>
          </>
        )}

        {message && <p className="auth-message">{message}</p>}
      </div>
    </div>
  );
}
