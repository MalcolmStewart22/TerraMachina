import WorldControls from "./components/WorldControls"
import WorldView from "./components/WorldView"



function App() {
  return (
    <div>
      <h1 className="text-5xl font-bold p-4">TerraMachina</h1>
      <div className="flex flex-row gap-4 p-4 h-screen">
        <div className="w-96">
          <WorldControls/>
        </div>
        <div className="flex-1">
          <WorldView/>
        </div>
      </div>
    </div>
  )
}
export default App