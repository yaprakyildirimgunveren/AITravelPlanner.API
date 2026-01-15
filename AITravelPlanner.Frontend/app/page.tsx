"use client";

import { useState } from "react";

type RecommendationItem = {
  destination: string;
  reason: string;
  estimatedCost: number;
  activities: string[];
};

type RecommendationResponse = {
  recommendations: RecommendationItem[];
};

const defaultForm = {
  userId: 1,
  from: "IST",
  to: "ATH",
  travelDate: "2026-05-10",
  budget: 1200,
  travelers: 1
};

export default function Home() {
  const [form, setForm] = useState(defaultForm);
  const [result, setResult] = useState<RecommendationResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const apiBaseUrl =
    process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5000";

  const handleChange = (key: keyof typeof defaultForm, value: string) => {
    setForm((prev) => ({
      ...prev,
      [key]: key === "budget" || key === "travelers" || key === "userId"
        ? Number(value)
        : value
    }));
  };

  const submit = async () => {
    setLoading(true);
    setError(null);
    setResult(null);

    try {
      const response = await fetch(`${apiBaseUrl}/api/Recommendation`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(form)
      });

      if (!response.ok) {
        throw new Error(`API error: ${response.status}`);
      }

      const data = (await response.json()) as RecommendationResponse;
      setResult(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container">
      <h1>AI Travel Planner</h1>
      <p>Get AI-driven recommendations and trigger the RabbitMQ flow.</p>

      <div className="grid">
        <div>
          <label>User Id</label>
          <input
            value={form.userId}
            onChange={(event) => handleChange("userId", event.target.value)}
          />
        </div>
        <div>
          <label>From</label>
          <input
            value={form.from}
            onChange={(event) => handleChange("from", event.target.value)}
          />
        </div>
        <div>
          <label>To</label>
          <input
            value={form.to}
            onChange={(event) => handleChange("to", event.target.value)}
          />
        </div>
        <div>
          <label>Travel date</label>
          <input
            type="date"
            value={form.travelDate}
            onChange={(event) => handleChange("travelDate", event.target.value)}
          />
        </div>
        <div>
          <label>Budget</label>
          <input
            value={form.budget}
            onChange={(event) => handleChange("budget", event.target.value)}
          />
        </div>
        <div>
          <label>Travelers</label>
          <input
            value={form.travelers}
            onChange={(event) => handleChange("travelers", event.target.value)}
          />
        </div>
      </div>

      <button onClick={submit} disabled={loading}>
        {loading ? "Loading..." : "Get Recommendations"}
      </button>

      {error && <div className="error">{error}</div>}

      {result && (
        <div className="card">
          <h2>Recommendations</h2>
          {result.recommendations.map((item, index) => (
            <div key={`${item.destination}-${index}`}>
              <h3>{item.destination}</h3>
              <p>{item.reason}</p>
              <p>Estimated cost: ${item.estimatedCost}</p>
              <p>Activities: {item.activities.join(", ")}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
