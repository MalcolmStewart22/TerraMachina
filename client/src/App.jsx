import { Routes, Route } from "react-router-dom"
import Landing from "./pages/Landing"
import WorldBuilder from "./pages/WorldBuilder"
import WorldSim from "./pages/WorldSim"
import WorldGen from "./pages/WorldGen"


function App() {
  return (
    <div className="bg-base">
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route path="/build" element={<WorldBuilder />} />
        <Route path="/sim" element={<WorldSim />} />
        <Route path="/gen" element={<WorldGen />} />
      </Routes>
    </div>
  )
}
export default App