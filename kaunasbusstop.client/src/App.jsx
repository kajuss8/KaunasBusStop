import './App.css';
import Home from './Pages/Home.jsx'
import { Route, Routes } from "react-router-dom"
import RouteSchedule from './Pages/RouteSchedule.jsx'

function App() {
    

    return (
        <>
            <div>
                <Routes>
                    <Route path='/' element={<Home />} />
                    <Route path='/RouteSchedule' element={<RouteSchedule />} />
                </Routes>
            </div>
        </>
    )
    
    
    
}

export default App;