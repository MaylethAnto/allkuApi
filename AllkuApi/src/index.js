const express = require('express');
const cors = require('cors');
const duenoRoutes = require('./routes/dueno.routes');
const caninoRoutes = require('./routes/canino.routes');
const administradorRoutes = require('./routes/administrador.routes');
const ejercicioRoutes = require('./routes/ejercicio.routes');
const gpsRoutes = require('./routes/gps.routes');
const historialClinicoRoutes = require('./routes/historial-clinico.routes');
const manejoPerfilesRoutes = require('./routes/manejo-perfiles.routes');
const paseadorRoutes = require('./routes/paseador.routes');
const recetaRoutes = require('./routes/receta.routes');

const app = express();

app.use(cors());
app.use(express.json());

// Routes
app.use('/api/duenos', duenoRoutes);
app.use('/api/caninos', caninoRoutes);
app.use('/api/administradores', administradorRoutes);
app.use('/api/ejercicios', ejercicioRoutes);
app.use('/api/gps', gpsRoutes);
app.use('/api/historial-clinico', historialClinicoRoutes);
app.use('/api/perfiles', manejoPerfilesRoutes);
app.use('/api/paseadores', paseadorRoutes);
app.use('/api/recetas', recetaRoutes);

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});