import "./globals.css";

export const metadata = {
  title: "AI Travel Planner",
  description: "AI recommendations and flight search"
};

export default function RootLayout({
  children
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
