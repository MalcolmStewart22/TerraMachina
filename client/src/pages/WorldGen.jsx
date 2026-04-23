import WorldControls from "../components/WorldControls"
import WorldView from "../components/WorldView"
import HeaderLinks from "../components/HeaderLinks"
import { useState, useEffect } from "react"

function WorldGen() {
  const [showModal, setShowModal] = useState(false)

  const handleDismiss = () => {
    sessionStorage.setItem("worldgen-modal-dismissed", "true")
    setShowModal(false)
  }
  
  useEffect(() => {
    const dismissed = sessionStorage.getItem("worldgen-modal-dismissed")
    if (!dismissed) {
      setShowModal(true)
    }
  }, [])
  return(
    <div className="bg-base text-ink">
      <HeaderLinks/>
      {showModal && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
          <div className="flex flex-col items-center justify-center bg-elevated border-accent border-2 rounded-2xl p-8 max-w-md mx-4 text-ink">
            <h2 className="text-4xl font-custom mb-4">Excuse the mess</h2>
            <p className="mb-6 text-muted font-custom text-xl">
              This area is in active development. The world generator pipeline is only partially working. You'll see geometry and ridges generate, but plate formation stops partway.
            </p>
            <button onClick={handleDismiss} className="bg-accent text-ink px-4 py-2 rounded-lg hover:bg-elevated border-accent border-2">
              Got it
            </button>
          </div>
        </div>
      )}
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

export default WorldGen