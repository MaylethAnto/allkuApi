const express = require('express');
const router = express.Router();
const { db } = require('../config/firebase-config');

// Obtener todas las recetas
router.get('/', async (req, res) => {
    try {
        const recetasSnapshot = await db.collection('recetas').get();
        const recetas = [];
        recetasSnapshot.forEach(doc => {
            recetas.push({ id: doc.id, ...doc.data() });
        });
        res.json(recetas);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Obtener una receta por ID
router.get('/:id', async (req, res) => {
    try {
        const recetaDoc = await db.collection('recetas').doc(req.params.id).get();
        if (!recetaDoc.exists) {
            return res.status(404).json({ message: 'Receta no encontrada' });
        }
        res.json({ id: recetaDoc.id, ...recetaDoc.data() });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Crear una nueva receta
router.post('/', async (req, res) => {
    try {
        const recetaData = req.body;
        const recetaRef = await db.collection('recetas').add(recetaData);
        res.status(201).json({ id: recetaRef.id, ...recetaData });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Actualizar una receta
router.put('/:id', async (req, res) => {
    try {
        const recetaRef = db.collection('recetas').doc(req.params.id);
        const receta = await recetaRef.get();
        if (!receta.exists) {
            return res.status(404).json({ message: 'Receta no encontrada' });
        }
        await recetaRef.update(req.body);
        res.json({ message: 'Receta actualizada exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Eliminar una receta
router.delete('/:id', async (req, res) => {
    try {
        const recetaRef = db.collection('recetas').doc(req.params.id);
        const receta = await recetaRef.get();
        if (!receta.exists) {
            return res.status(404).json({ message: 'Receta no encontrada' });
        }
        await recetaRef.delete();
        res.json({ message: 'Receta eliminada exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

module.exports = router;
