const express = require('express');
const router = express.Router();
const { db } = require('../config/firebase-config');

// Obtener todos los paseadores
router.get('/', async (req, res) => {
    try {
        const paseadoresSnapshot = await db.collection('paseadores').get();
        const paseadores = [];
        paseadoresSnapshot.forEach(doc => {
            paseadores.push({ id: doc.id, ...doc.data() });
        });
        res.json(paseadores);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Obtener un paseador por ID
router.get('/:id', async (req, res) => {
    try {
        const paseadorDoc = await db.collection('paseadores').doc(req.params.id).get();
        if (!paseadorDoc.exists) {
            return res.status(404).json({ message: 'Paseador no encontrado' });
        }
        res.json({ id: paseadorDoc.id, ...paseadorDoc.data() });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Crear un nuevo paseador
router.post('/', async (req, res) => {
    try {
        const paseadorData = req.body;
        const paseadorRef = await db.collection('paseadores').add(paseadorData);
        res.status(201).json({ id: paseadorRef.id, ...paseadorData });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Actualizar un paseador
router.put('/:id', async (req, res) => {
    try {
        const paseadorRef = db.collection('paseadores').doc(req.params.id);
        const paseador = await paseadorRef.get();
        if (!paseador.exists) {
            return res.status(404).json({ message: 'Paseador no encontrado' });
        }
        await paseadorRef.update(req.body);
        res.json({ message: 'Paseador actualizado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Eliminar un paseador
router.delete('/:id', async (req, res) => {
    try {
        const paseadorRef = db.collection('paseadores').doc(req.params.id);
        const paseador = await paseadorRef.get();
        if (!paseador.exists) {
            return res.status(404).json({ message: 'Paseador no encontrado' });
        }
        await paseadorRef.delete();
        res.json({ message: 'Paseador eliminado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

module.exports = router;
