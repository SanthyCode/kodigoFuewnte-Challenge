// frontend/src/App.tsx
import './App.scss';
import { SeparataForm } from './components/SeparataForm/SeparataForm';

function App() {
  return (
    <div className="app-layout">
      <header className="header">
        <h1>Kódigo Fuente</h1>
        <p>Módulo de Gestión de Ofertas y Separatas</p>
      </header>
      
      <main className="main-content">
        {/* Aquí renderizamos el componente que construimos previamente */}
        <SeparataForm />
      </main>
    </div>
  );
}

export default App;