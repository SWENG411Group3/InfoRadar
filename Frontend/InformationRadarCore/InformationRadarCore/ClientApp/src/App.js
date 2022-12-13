import { BrowserRouter, Routes, Route } from "react-router-dom";

import { useContext } from "react";
import { DarkModeContext } from "./context/darkModeContext";
import List from "./pages/list/List";

function App() {
    const { darkMode } = useContext(DarkModeContext);

    return (
        <div className={darkMode ? "app dark" : "app"}>
            <BrowserRouter>
                <Routes>
                    <Route path="/" >
                        <Route path="/" element={<List />}>
                            <Route
                                index
                            />
                        </Route>
                    </Route>
                </Routes>
            </BrowserRouter>
        </div>
    );
}

export default App;